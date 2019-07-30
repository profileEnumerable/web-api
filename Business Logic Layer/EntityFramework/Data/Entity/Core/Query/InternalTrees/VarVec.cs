// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.VarVec
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal class VarVec : IEnumerable<Var>, IEnumerable
  {
    private readonly BitArray m_bitVector;
    private readonly Command m_command;

    internal void Clear()
    {
      this.m_bitVector.Length = 0;
    }

    internal void And(VarVec other)
    {
      this.Align(other);
      this.m_bitVector.And(other.m_bitVector);
    }

    internal void Or(VarVec other)
    {
      this.Align(other);
      this.m_bitVector.Or(other.m_bitVector);
    }

    internal void Minus(VarVec other)
    {
      VarVec varVec = this.m_command.CreateVarVec(other);
      varVec.m_bitVector.Length = this.m_bitVector.Length;
      varVec.m_bitVector.Not();
      this.And(varVec);
      this.m_command.ReleaseVarVec(varVec);
    }

    internal bool Overlaps(VarVec other)
    {
      VarVec varVec = this.m_command.CreateVarVec(other);
      varVec.And(this);
      bool flag = !varVec.IsEmpty;
      this.m_command.ReleaseVarVec(varVec);
      return flag;
    }

    internal bool Subsumes(VarVec other)
    {
      for (int index = 0; index < other.m_bitVector.Length; ++index)
      {
        if (other.m_bitVector[index] && (index >= this.m_bitVector.Length || !this.m_bitVector[index]))
          return false;
      }
      return true;
    }

    internal void InitFrom(VarVec other)
    {
      this.Clear();
      this.m_bitVector.Length = other.m_bitVector.Length;
      this.m_bitVector.Or(other.m_bitVector);
    }

    internal void InitFrom(IEnumerable<Var> other)
    {
      this.InitFrom(other, false);
    }

    internal void InitFrom(IEnumerable<Var> other, bool ignoreParameters)
    {
      this.Clear();
      foreach (Var v in other)
      {
        if (!ignoreParameters || v.VarType != VarType.Parameter)
          this.Set(v);
      }
    }

    public IEnumerator<Var> GetEnumerator()
    {
      return (IEnumerator<Var>) this.m_command.GetVarVecEnumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    internal int Count
    {
      get
      {
        int num = 0;
        foreach (Var var in this)
          ++num;
        return num;
      }
    }

    internal bool IsSet(Var v)
    {
      this.Align(v.Id);
      return this.m_bitVector.Get(v.Id);
    }

    internal void Set(Var v)
    {
      this.Align(v.Id);
      this.m_bitVector.Set(v.Id, true);
    }

    internal void Clear(Var v)
    {
      this.Align(v.Id);
      this.m_bitVector.Set(v.Id, false);
    }

    internal bool IsEmpty
    {
      get
      {
        return this.First == null;
      }
    }

    internal Var First
    {
      get
      {
        using (IEnumerator<Var> enumerator = this.GetEnumerator())
        {
          if (enumerator.MoveNext())
            return enumerator.Current;
        }
        return (Var) null;
      }
    }

    internal VarVec Remap(Dictionary<Var, Var> varMap)
    {
      VarVec varVec = this.m_command.CreateVarVec();
      foreach (Var key in this)
      {
        Var v;
        if (!varMap.TryGetValue(key, out v))
          v = key;
        varVec.Set(v);
      }
      return varVec;
    }

    internal VarVec(Command command)
    {
      this.m_bitVector = new BitArray(64);
      this.m_command = command;
    }

    private void Align(VarVec other)
    {
      if (other.m_bitVector.Length == this.m_bitVector.Length)
        return;
      if (other.m_bitVector.Length > this.m_bitVector.Length)
        this.m_bitVector.Length = other.m_bitVector.Length;
      else
        other.m_bitVector.Length = this.m_bitVector.Length;
    }

    private void Align(int idx)
    {
      if (idx < this.m_bitVector.Length)
        return;
      this.m_bitVector.Length = idx + 1;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      string str = string.Empty;
      foreach (Var var in this)
      {
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) str, (object) var.Id);
        str = ",";
      }
      return stringBuilder.ToString();
    }

    public VarVec Clone()
    {
      VarVec varVec = this.m_command.CreateVarVec();
      varVec.InitFrom(this);
      return varVec;
    }

    internal class VarVecEnumerator : IEnumerator<Var>, IEnumerator, IDisposable
    {
      private int m_position;
      private Command m_command;
      private BitArray m_bitArray;

      internal VarVecEnumerator(VarVec vec)
      {
        this.Init(vec);
      }

      internal void Init(VarVec vec)
      {
        this.m_position = -1;
        this.m_command = vec.m_command;
        this.m_bitArray = vec.m_bitVector;
      }

      public Var Current
      {
        get
        {
          if (this.m_position < 0 || this.m_position >= this.m_bitArray.Length)
            return (Var) null;
          return this.m_command.GetVar(this.m_position);
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.Current;
        }
      }

      public bool MoveNext()
      {
        for (++this.m_position; this.m_position < this.m_bitArray.Length; ++this.m_position)
        {
          if (this.m_bitArray[this.m_position])
            return true;
        }
        return false;
      }

      public void Reset()
      {
        this.m_position = -1;
      }

      public void Dispose()
      {
        GC.SuppressFinalize((object) this);
        this.m_bitArray = (BitArray) null;
        this.m_command.ReleaseVarVecEnumerator(this);
      }
    }
  }
}
