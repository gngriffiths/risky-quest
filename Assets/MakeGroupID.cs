
using System.Collections.Generic;

public static class MakeGroupID
{
    static int id;
    
    public static int NewID()
    {
        return id++;
    }
}
