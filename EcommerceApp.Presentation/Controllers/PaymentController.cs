using EcommerceApp.Application.DTO.Requests;
using EcommerceApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;
using System.Security.Claims;

namespace EcommerceApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        public readonly ILogger<PaymentController> _logger;
        private readonly string _webHooksSecret;
        private readonly IOrderService _orderService;

        public PaymentController(IConfiguration configuration, ILogger<PaymentController> logger, IOrderService orderService)
        {
            _webHooksSecret = configuration["Stripe:WebhookSecret"]!;
            _logger = logger;
            _orderService = orderService;
        }

        [HttpPost("Create-intent")]
        [Authorize]
        public IActionResult CreatePaymentIntent([FromBody] PaymentRequestDTO request)
        {
            try
            {
                var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var jsonItems = JsonConvert.SerializeObject(request.Items);

                var options = new PaymentIntentCreateOptions
                {
                    Amount = Convert.ToInt64(Math.Round(request.Items.Sum(i => i.Quantity * i.Price), MidpointRounding.AwayFromZero)),
                    Currency = "vnd",
                    PaymentMethodTypes = new List<string> { "card" },
                    Metadata = new Dictionary<string, string>
                    {
                        { "userId", userId },
                        { "items", jsonItems }
                    }
                };

                var service = new PaymentIntentService();
                var paymentIntent = service.Create(options);

                return Ok(new { paymentIntent.ClientSecret });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("Webhook")]
        public async Task<IActionResult> HandleWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _webHooksSecret
                );
                if (stripeEvent.Type == EventTypes.PaymentIntentCreated)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                }
                else if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    if (paymentIntent.Metadata.TryGetValue("items", out var itemsData) && paymentIntent.Metadata.TryGetValue("userId", out var userId))
                    {
                        var orderItems = JsonConvert.DeserializeObject<List<OrderItemRequestDTO>>(itemsData);
                        await _orderService.CreateOrderAsync(userId, paymentIntent.Created, orderItems!);
                    }
                }
                else if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    var errorMessage = paymentIntent.LastPaymentError?.Message;
                    _logger.LogError($"Payment failed for PaymentIntent: {paymentIntent.Id}, Error: {errorMessage}");
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }
    }
}
