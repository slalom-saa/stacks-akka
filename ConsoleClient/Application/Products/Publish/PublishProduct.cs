using Slalom.Stacks.Messaging;

namespace ConsoleClient.Application.Products.Publish
{
    [Path("products/publish")]
    public class PublishProduct : UseCase<PublishProductCommand>
    {
        public override void Execute(PublishProductCommand message)
        {
        }
    }
}
