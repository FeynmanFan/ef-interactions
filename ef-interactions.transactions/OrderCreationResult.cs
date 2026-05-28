using ef_interactions.nplusone;

namespace ef_interactions.transactions
{
    public class OrderCreationResult
    {
        public bool Success { get; set; }

        public Order? Result {  get; set; }

        public static OrderCreationResult Failure
        {
            get
            {
                return new OrderCreationResult
                {
                    Success = false,
                    Result = null
                };
            }
        }
    }
}