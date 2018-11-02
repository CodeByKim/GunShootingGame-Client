using System;
using System.Threading;

public class Locker
{
    private Object key;

    public void Initialize()
    {
        this.key = new Object();
    }

    public void Lock()
    {
        Monitor.Enter(this.key);
    }

    public void Unlock()
    {
        Monitor.Exit(this.key);
    }
}
