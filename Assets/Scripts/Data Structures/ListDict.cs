using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public abstract class ListDict<Key, Value>
{
    public List<Key> keys = new List<Key>();
    public List<Value> values = new List<Value>();
    
    public int Count
    {
        get
        {
            return keys.Count;
        }
    }

    public Value this[Key key]
    {
        get
        {
            for(int i = 0; i < keys.Count; i++)
            {
                if (keys[i].Equals(key))
                {
                    return values[i];
                }
            }
            return default(Value);
        }
        set
        {
            for(int i = 0; i < keys.Count; i++)
            {
                if (keys[i].Equals(key))
                {
                    values[i] = value; //ugh...
                    return;
                }
            }
            keys.Add(key);
            values.Add(value);
        }
    }
    public void Clear()
    {
        keys.Clear();
        values.Clear();
    }

    public bool TryGetValue(Key key, out Value value)
    {
        bool succeeded = false;
        for(int i = 0; i < keys.Count; i++)
        {
            if (keys[i].Equals(key))
            {
                value = values[i];
                succeeded = true;
                break;
            }
        }
        value = default(Value);
        return succeeded;
    }

    public void Add(Key key, Value value)
    {
        this[key] = value;
    }

    public IEnumerable<Value> Values
    {
        get
        {
            foreach (Value value in values)
            {
                yield return value;
            }
        }
    }
}
