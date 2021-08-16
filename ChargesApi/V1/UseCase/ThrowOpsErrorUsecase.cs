
namespace ChargesApi.V1.UseCase
{
    public static class ThrowOpsErrorUseCase
    {
        public static void Execute()
        {
            throw new TestOpsErrorException();
        }
    }
}
