using System;
using System.Collections;
using System.Collections.Generic;

public class RecognisedShapesList : IList<RecognisedShape>
{
    private List<RecognisedShape> _shapes;

    public RecognisedShapesList()
    {
        _shapes = new List<RecognisedShape>();
    }

    public RecognisedShape GetShape(int index)
    {
        if (index >= _shapes.Count)
        {
            throw new ArgumentOutOfRangeException("index");
        }

        return _shapes[index];
    }

    public RecognisedShape this[int index]
    {
        get
        {
            return GetShape(index);
        }

        set
        {
            if (index >= _shapes.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            _shapes[index] = value;
        }
    }

    public int Count
    {
        get
        {
            return _shapes.Count;
        }
    }

    public bool IsReadOnly
    {
        get
        {
            return false;
        }
    }

    public void Add(RecognisedShape item)
    {
        _shapes.Add(item);
    }

    public void Clear()
    {
        _shapes.Clear();
    }

    public bool Contains(RecognisedShape item)
    {
        return _shapes.Contains(item);
    }

    public void CopyTo(RecognisedShape[] array, int arrayIndex)
    {
        _shapes.CopyTo(array, arrayIndex);
    }

    public IEnumerator<RecognisedShape> GetEnumerator()
    {
        return _shapes.GetEnumerator();
    }

    public int IndexOf(RecognisedShape item)
    {
        return _shapes.IndexOf(item);
    }

    public void Insert(int index, RecognisedShape item)
    {
        _shapes.Insert(index, item);
    }

    public bool Remove(RecognisedShape item)
    {
        return _shapes.Remove(item);
    }

    public void RemoveAt(int index)
    {
        _shapes.RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _shapes.GetEnumerator();
    }
}
