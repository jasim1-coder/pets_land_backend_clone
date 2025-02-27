using Pet_s_Land.DTOs;
using Pet_s_Land.Repositories;
using System.Threading.Tasks;

namespace Pet_s_Land.Services
{
    public interface IPaymentService
    {
        Task<ResponseDto<string>> RazorOrderCreate(long price);
        Task<ResponseDto<bool>> RazorPayment(PaymentDto payment);
        Task<ResponseDto<bool>> CreateOrder(int userId, CreateOrderDto createOrderDTO);

        Task<ResponseDto<bool>> PlaceOrder(int userId,  CreateOrderDto createOrderDTO);

        Task<ResponseDto<List<ViewOrderUserDetailDto>>> GetUserOrders(int userId);

    }

    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepo _paymentRepo;

        public PaymentService(IPaymentRepo paymentRepo)
        {
            _paymentRepo = paymentRepo;
        }

        public async Task<ResponseDto<string>> RazorOrderCreate(long price)
        {
            return await _paymentRepo.RazorOrderCreate(price);
        }

        public async Task<ResponseDto<bool>> RazorPayment(PaymentDto payment)
        {
            return await _paymentRepo.RazorPayment(payment);
        }

        public async Task<ResponseDto<bool>> CreateOrder(int userId, CreateOrderDto createOrderDTO)
        {
            return await _paymentRepo.CreateOrder(userId, createOrderDTO);
        }
        public async Task<ResponseDto<bool>> PlaceOrder(int userId,CreateOrderDto createOrderDTO)
        {
            return await _paymentRepo.PlaceOrder(userId, createOrderDTO);
        }

        public async Task<ResponseDto<List<ViewOrderUserDetailDto>>> GetUserOrders(int userId)
        {
           return await _paymentRepo.GetUserOrders(userId);
        }

    }
}
