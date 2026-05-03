namespace OODGame.Actions
{
    public interface IAction<out TResult>
    {
        TResult Execute();
    }
}