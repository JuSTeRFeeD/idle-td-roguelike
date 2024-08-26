using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems
{
    public class ViewEntityDisposableInitializer : IInitializer
    {
        public World World { get; set; }

        public void OnAwake()
        {
            World.GetStash<ViewEntity>().AsDisposable();
        }

        public void Dispose()
        {
        }
    }
}