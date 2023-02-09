using System.Collections.Generic;

public static class ListPool<T>
{
    private static Stack<List<T>> _stack = new Stack<List<T>>();

    public static List<T> Get()
    {
        if (_stack.Count > 0)
        {
            return _stack.Pop();
        }

        return new List<T>();
    }
    
    public static void Add(List<T> list)
    {
        list.Clear();
        _stack.Push(list);
    }
    
}
