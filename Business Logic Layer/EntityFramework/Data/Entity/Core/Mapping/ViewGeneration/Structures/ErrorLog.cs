// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Structures.ErrorLog
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Structures
{
  internal class ErrorLog : InternalBase
  {
    private readonly List<ErrorLog.Record> m_log;

    internal ErrorLog()
    {
      this.m_log = new List<ErrorLog.Record>();
    }

    internal int Count
    {
      get
      {
        return this.m_log.Count;
      }
    }

    internal IEnumerable<EdmSchemaError> Errors
    {
      get
      {
        foreach (ErrorLog.Record record in this.m_log)
          yield return record.Error;
      }
    }

    internal void AddEntry(ErrorLog.Record record)
    {
      this.m_log.Add(record);
    }

    internal void Merge(ErrorLog log)
    {
      foreach (ErrorLog.Record record in log.m_log)
        this.m_log.Add(record);
    }

    internal void PrintTrace()
    {
      StringBuilder builder = new StringBuilder();
      this.ToCompactString(builder);
      Helpers.StringTraceLine(builder.ToString());
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      foreach (InternalBase internalBase in this.m_log)
        internalBase.ToCompactString(builder);
    }

    internal string ToUserString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (ErrorLog.Record record in this.m_log)
      {
        string userString = record.ToUserString();
        stringBuilder.AppendLine(userString);
      }
      return stringBuilder.ToString();
    }

    internal class Record : InternalBase
    {
      private EdmSchemaError m_mappingError;
      private List<Cell> m_sourceCells;
      private string m_debugMessage;

      internal Record(
        ViewGenErrorCode errorCode,
        string message,
        IEnumerable<LeftCellWrapper> wrappers,
        string debugMessage)
      {
        IEnumerable<Cell> cellsForWrappers = LeftCellWrapper.GetInputCellsForWrappers(wrappers);
        this.Init(errorCode, message, cellsForWrappers, debugMessage);
      }

      internal Record(
        ViewGenErrorCode errorCode,
        string message,
        Cell sourceCell,
        string debugMessage)
      {
        this.Init(errorCode, message, (IEnumerable<Cell>) new Cell[1]
        {
          sourceCell
        }, debugMessage);
      }

      internal Record(
        ViewGenErrorCode errorCode,
        string message,
        IEnumerable<Cell> sourceCells,
        string debugMessage)
      {
        this.Init(errorCode, message, sourceCells, debugMessage);
      }

      internal Record(EdmSchemaError error)
      {
        this.m_debugMessage = error.ToString();
        this.m_mappingError = error;
      }

      private void Init(
        ViewGenErrorCode errorCode,
        string message,
        IEnumerable<Cell> sourceCells,
        string debugMessage)
      {
        this.m_sourceCells = new List<Cell>(sourceCells);
        CellLabel cellLabel = this.m_sourceCells[0].CellLabel;
        string sourceLocation = cellLabel.SourceLocation;
        int startLineNumber = cellLabel.StartLineNumber;
        int startLinePosition = cellLabel.StartLinePosition;
        string message1 = ErrorLog.Record.InternalToString(message, debugMessage, this.m_sourceCells, errorCode, false);
        this.m_debugMessage = ErrorLog.Record.InternalToString(message, debugMessage, this.m_sourceCells, errorCode, true);
        this.m_mappingError = new EdmSchemaError(message1, (int) errorCode, EdmSchemaErrorSeverity.Error, sourceLocation, startLineNumber, startLinePosition);
      }

      [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
      internal EdmSchemaError Error
      {
        get
        {
          return this.m_mappingError;
        }
      }

      internal override void ToCompactString(StringBuilder builder)
      {
        builder.Append(this.m_debugMessage);
      }

      private static void GetUserLinesFromCells(
        IEnumerable<Cell> sourceCells,
        StringBuilder lineBuilder,
        bool isInvariant)
      {
        IOrderedEnumerable<Cell> orderedEnumerable = sourceCells.OrderBy<Cell, int>((Func<Cell, int>) (cell => cell.CellLabel.StartLineNumber), (IComparer<int>) Comparer<int>.Default);
        bool flag = true;
        foreach (Cell cell in (IEnumerable<Cell>) orderedEnumerable)
        {
          if (!flag)
            lineBuilder.Append(isInvariant ? EntityRes.GetString("ViewGen_CommaBlank") : ", ");
          flag = false;
          lineBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) cell.CellLabel.StartLineNumber);
        }
      }

      private static string InternalToString(
        string message,
        string debugMessage,
        List<Cell> sourceCells,
        ViewGenErrorCode errorCode,
        bool isInvariant)
      {
        StringBuilder builder = new StringBuilder();
        if (isInvariant)
        {
          builder.AppendLine(debugMessage);
          builder.Append(isInvariant ? "ERROR" : Strings.ViewGen_Error);
          StringUtil.FormatStringBuilder(builder, " ({0}): ", (object) (int) errorCode);
        }
        StringBuilder lineBuilder = new StringBuilder();
        ErrorLog.Record.GetUserLinesFromCells((IEnumerable<Cell>) sourceCells, lineBuilder, isInvariant);
        if (isInvariant)
        {
          if (sourceCells.Count > 1)
            StringUtil.FormatStringBuilder(builder, "Problem in Mapping Fragments starting at lines {0}: ", (object) lineBuilder.ToString());
          else
            StringUtil.FormatStringBuilder(builder, "Problem in Mapping Fragment starting at line {0}: ", (object) lineBuilder.ToString());
        }
        else if (sourceCells.Count > 1)
          builder.Append(Strings.ViewGen_ErrorLog2((object) lineBuilder.ToString()));
        else
          builder.Append(Strings.ViewGen_ErrorLog((object) lineBuilder.ToString()));
        builder.AppendLine(message);
        return builder.ToString();
      }

      internal string ToUserString()
      {
        return this.m_mappingError.ToString();
      }
    }
  }
}
