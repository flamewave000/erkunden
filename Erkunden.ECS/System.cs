namespace Erkunden.ECS
{
	public interface System<TData>
	{
		void Process(Entity entity, TData data);
	}
}
