namespace Erkunden.ECS
{
	public interface ISystem<TData>
	{
		void Process(Entity[] entities, TData data);
	}
}
