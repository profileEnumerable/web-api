// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.EntityKey
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace System.Data.Entity.Core
{
  /// <summary>An identifier for an entity.</summary>
  [DataContract(IsReference = true)]
  [DebuggerDisplay("{ConcatKeyValue()}")]
  [Serializable]
  public sealed class EntityKey : IEquatable<EntityKey>
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    private static readonly EntityKey _noEntitySetKey = new EntityKey("NoEntitySetKey.NoEntitySetKey");
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    private static readonly EntityKey _entityNotValidKey = new EntityKey("EntityNotValidKey.EntityNotValidKey");
    private static readonly ConcurrentDictionary<string, string> NameLookup = new ConcurrentDictionary<string, string>();
    private string _entitySetName;
    private string _entityContainerName;
    private object _singletonKeyValue;
    private object[] _compositeKeyValues;
    private string[] _keyNames;
    private readonly bool _isLocked;
    [NonSerialized]
    private bool _containsByteArray;
    [NonSerialized]
    private EntityKeyMember[] _deserializedMembers;
    [NonSerialized]
    private int _hashCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.EntityKey" /> class.
    /// </summary>
    public EntityKey()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.EntityKey" /> class with an entity set name and a generic
    /// <see cref="T:System.Collections.Generic.KeyValuePair" />
    /// collection.
    /// </summary>
    /// <param name="qualifiedEntitySetName">
    /// A <see cref="T:System.String" /> that is the entity set name qualified by the entity container name.
    /// </param>
    /// <param name="entityKeyValues">
    /// A generic <see cref="T:System.Collections.Generic.KeyValuePair" /> collection.Each key/value pair has a property name as the key and the value of that property as the value. There should be one pair for each property that is part of the
    /// <see cref="T:System.Data.Entity.Core.EntityKey" />
    /// . The order of the key/value pairs is not important, but each key property should be included. The property names are simple names that are not qualified with an entity type name or the schema name.
    /// </param>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public EntityKey(
      string qualifiedEntitySetName,
      IEnumerable<KeyValuePair<string, object>> entityKeyValues)
    {
      Check.NotEmpty(qualifiedEntitySetName, nameof (qualifiedEntitySetName));
      Check.NotNull<IEnumerable<KeyValuePair<string, object>>>(entityKeyValues, nameof (entityKeyValues));
      this.InitializeEntitySetName(qualifiedEntitySetName);
      this.InitializeKeyValues(entityKeyValues, false, false);
      this._isLocked = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.EntityKey" /> class with an entity set name and an
    /// <see cref="T:System.Collections.Generic.IEnumerable`1" />
    /// collection of
    /// <see cref="T:System.Data.Entity.Core.EntityKeyMember" />
    /// objects.
    /// </summary>
    /// <param name="qualifiedEntitySetName">
    /// A <see cref="T:System.String" /> that is the entity set name qualified by the entity container name.
    /// </param>
    /// <param name="entityKeyValues">
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1" /> collection of
    /// <see cref="T:System.Data.Entity.Core.EntityKeyMember" />
    /// objects with which to initialize the key.
    /// </param>
    public EntityKey(string qualifiedEntitySetName, IEnumerable<EntityKeyMember> entityKeyValues)
    {
      Check.NotEmpty(qualifiedEntitySetName, nameof (qualifiedEntitySetName));
      Check.NotNull<IEnumerable<EntityKeyMember>>(entityKeyValues, nameof (entityKeyValues));
      this.InitializeEntitySetName(qualifiedEntitySetName);
      this.InitializeKeyValues((IEnumerable<KeyValuePair<string, object>>) new EntityKey.KeyValueReader(entityKeyValues), false, false);
      this._isLocked = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.EntityKey" /> class with an entity set name and specific entity key pair.
    /// </summary>
    /// <param name="qualifiedEntitySetName">
    /// A <see cref="T:System.String" /> that is the entity set name qualified by the entity container name.
    /// </param>
    /// <param name="keyName">
    /// A <see cref="T:System.String" /> that is the name of the key.
    /// </param>
    /// <param name="keyValue">
    /// An <see cref="T:System.Object" /> that is the key value.
    /// </param>
    public EntityKey(string qualifiedEntitySetName, string keyName, object keyValue)
    {
      Check.NotEmpty(qualifiedEntitySetName, nameof (qualifiedEntitySetName));
      Check.NotEmpty(keyName, nameof (keyName));
      Check.NotNull<object>(keyValue, nameof (keyValue));
      this.InitializeEntitySetName(qualifiedEntitySetName);
      EntityKey.ValidateName(keyName);
      this._keyNames = new string[1]{ keyName };
      this._singletonKeyValue = keyValue;
      this._isLocked = true;
    }

    internal EntityKey(EntitySet entitySet, IExtendedDataRecord record)
    {
      this._entitySetName = entitySet.Name;
      this._entityContainerName = entitySet.EntityContainer.Name;
      this.InitializeKeyValues(entitySet, record);
      this._isLocked = true;
    }

    internal EntityKey(string qualifiedEntitySetName)
    {
      this.InitializeEntitySetName(qualifiedEntitySetName);
      this._isLocked = true;
    }

    internal EntityKey(EntitySetBase entitySet)
    {
      this._entitySetName = entitySet.Name;
      this._entityContainerName = entitySet.EntityContainer.Name;
      this._isLocked = true;
    }

    internal EntityKey(EntitySetBase entitySet, object singletonKeyValue)
    {
      this._singletonKeyValue = singletonKeyValue;
      this._entitySetName = entitySet.Name;
      this._entityContainerName = entitySet.EntityContainer.Name;
      this._keyNames = entitySet.ElementType.KeyMemberNames;
      this._isLocked = true;
    }

    internal EntityKey(EntitySetBase entitySet, object[] compositeKeyValues)
    {
      this._compositeKeyValues = compositeKeyValues;
      this._entitySetName = entitySet.Name;
      this._entityContainerName = entitySet.EntityContainer.Name;
      this._keyNames = entitySet.ElementType.KeyMemberNames;
      this._isLocked = true;
    }

    /// <summary>
    /// Gets a singleton EntityKey by which a read-only entity is identified.
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    public static EntityKey NoEntitySetKey
    {
      get
      {
        return EntityKey._noEntitySetKey;
      }
    }

    /// <summary>
    /// Gets a singleton EntityKey identifying an entity resulted from a failed TREAT.
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    public static EntityKey EntityNotValidKey
    {
      get
      {
        return EntityKey._entityNotValidKey;
      }
    }

    /// <summary>Gets or sets the name of the entity set.</summary>
    /// <returns>
    /// A <see cref="T:System.String" /> value that is the name of the entity set for the entity to which the
    /// <see cref="T:System.Data.Entity.Core.EntityKey" />
    /// belongs.
    /// </returns>
    [DataMember]
    public string EntitySetName
    {
      get
      {
        return this._entitySetName;
      }
      set
      {
        this.ValidateWritable((object) this._entitySetName);
        this._entitySetName = EntityKey.LookupSingletonName(value);
      }
    }

    /// <summary>Gets or sets the name of the entity container.</summary>
    /// <returns>
    /// A <see cref="T:System.String" /> value that is the name of the entity container for the entity to which the
    /// <see cref="T:System.Data.Entity.Core.EntityKey" />
    /// belongs.
    /// </returns>
    [DataMember]
    public string EntityContainerName
    {
      get
      {
        return this._entityContainerName;
      }
      set
      {
        this.ValidateWritable((object) this._entityContainerName);
        this._entityContainerName = EntityKey.LookupSingletonName(value);
      }
    }

    /// <summary>
    /// Gets or sets the key values associated with this <see cref="T:System.Data.Entity.Core.EntityKey" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of key values for this
    /// <see cref="T:System.Data.Entity.Core.EntityKey" />
    /// .
    /// </returns>
    [DataMember]
    [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Required for this feature")]
    public EntityKeyMember[] EntityKeyValues
    {
      get
      {
        if (this.IsTemporary)
          return (EntityKeyMember[]) null;
        EntityKeyMember[] entityKeyMemberArray;
        if (this._singletonKeyValue != null)
        {
          entityKeyMemberArray = new EntityKeyMember[1]
          {
            new EntityKeyMember(this._keyNames[0], this._singletonKeyValue)
          };
        }
        else
        {
          entityKeyMemberArray = new EntityKeyMember[this._compositeKeyValues.Length];
          for (int index = 0; index < this._compositeKeyValues.Length; ++index)
            entityKeyMemberArray[index] = new EntityKeyMember(this._keyNames[index], this._compositeKeyValues[index]);
        }
        return entityKeyMemberArray;
      }
      set
      {
        this.ValidateWritable((object) this._keyNames);
        if (value == null || this.InitializeKeyValues((IEnumerable<KeyValuePair<string, object>>) new EntityKey.KeyValueReader((IEnumerable<EntityKeyMember>) value), true, true))
          return;
        this._deserializedMembers = value;
      }
    }

    /// <summary>
    /// Gets a value that indicates whether the <see cref="T:System.Data.Entity.Core.EntityKey" /> is temporary.
    /// </summary>
    /// <returns>
    /// true if the <see cref="T:System.Data.Entity.Core.EntityKey" /> is temporary; otherwise, false.
    /// </returns>
    public bool IsTemporary
    {
      get
      {
        if (this.SingletonKeyValue == null)
          return this.CompositeKeyValues == null;
        return false;
      }
    }

    private object SingletonKeyValue
    {
      get
      {
        if (this.RequiresDeserialization)
          this.DeserializeMembers();
        return this._singletonKeyValue;
      }
    }

    private object[] CompositeKeyValues
    {
      get
      {
        if (this.RequiresDeserialization)
          this.DeserializeMembers();
        return this._compositeKeyValues;
      }
    }

    /// <summary>Gets the entity set for this entity key from the given metadata workspace.</summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" /> for the entity key.
    /// </returns>
    /// <param name="metadataWorkspace">The metadata workspace that contains the entity.</param>
    /// <exception cref="T:System.ArgumentException">The entity set could not be located in the specified metadata workspace.</exception>
    public EntitySet GetEntitySet(MetadataWorkspace metadataWorkspace)
    {
      Check.NotNull<MetadataWorkspace>(metadataWorkspace, nameof (metadataWorkspace));
      if (string.IsNullOrEmpty(this._entityContainerName) || string.IsNullOrEmpty(this._entitySetName))
        throw new InvalidOperationException(Strings.EntityKey_MissingEntitySetName);
      return metadataWorkspace.GetEntityContainer(this._entityContainerName, DataSpace.CSpace).GetEntitySetByName(this._entitySetName, false);
    }

    /// <summary>Returns a value that indicates whether this instance is equal to a specified object. </summary>
    /// <returns>true if this instance and  obj  have equal values; otherwise, false. </returns>
    /// <param name="obj">
    /// An <see cref="T:System.Object" /> to compare with this instance.
    /// </param>
    public override bool Equals(object obj)
    {
      return EntityKey.InternalEquals(this, obj as EntityKey, true);
    }

    /// <summary>
    /// Returns a value that indicates whether this instance is equal to a specified
    /// <see cref="T:System.Data.Entity.Core.EntityKey" />
    /// .
    /// </summary>
    /// <returns>true if this instance and  other  have equal values; otherwise, false. </returns>
    /// <param name="other">
    /// An <see cref="T:System.Data.Entity.Core.EntityKey" /> object to compare with this instance.
    /// </param>
    public bool Equals(EntityKey other)
    {
      return EntityKey.InternalEquals(this, other, true);
    }

    /// <summary>
    /// Serves as a hash function for the current <see cref="T:System.Data.Entity.Core.EntityKey" /> object.
    /// <see cref="M:System.Data.Entity.Core.EntityKey.GetHashCode" />
    /// is suitable for hashing algorithms and data structures such as a hash table.
    /// </summary>
    /// <returns>
    /// A hash code for the current <see cref="T:System.Data.Entity.Core.EntityKey" />.
    /// </returns>
    public override int GetHashCode()
    {
      int hashCode = this._hashCode;
      if (hashCode == 0)
      {
        this._containsByteArray = false;
        if (this.RequiresDeserialization)
          this.DeserializeMembers();
        if (this._entitySetName != null)
          hashCode = this._entitySetName.GetHashCode();
        if (this._entityContainerName != null)
          hashCode ^= this._entityContainerName.GetHashCode();
        if (this._singletonKeyValue != null)
          hashCode = this.AddHashValue(hashCode, this._singletonKeyValue);
        else if (this._compositeKeyValues != null)
        {
          int index = 0;
          for (int length = this._compositeKeyValues.Length; index < length; ++index)
            hashCode = this.AddHashValue(hashCode, this._compositeKeyValues[index]);
        }
        else
          hashCode = base.GetHashCode();
        if (this._isLocked || !string.IsNullOrEmpty(this._entitySetName) && !string.IsNullOrEmpty(this._entityContainerName) && (this._singletonKeyValue != null || this._compositeKeyValues != null))
          this._hashCode = hashCode;
      }
      return hashCode;
    }

    private int AddHashValue(int hashCode, object keyValue)
    {
      byte[] bytes = keyValue as byte[];
      if (bytes == null)
        return hashCode ^ keyValue.GetHashCode();
      hashCode ^= ByValueEqualityComparer.ComputeBinaryHashCode(bytes);
      this._containsByteArray = true;
      return hashCode;
    }

    /// <summary>
    /// Compares two <see cref="T:System.Data.Entity.Core.EntityKey" /> objects.
    /// </summary>
    /// <returns>true if the  key1  and  key2  values are equal; otherwise, false.</returns>
    /// <param name="key1">
    /// A <see cref="T:System.Data.Entity.Core.EntityKey" /> to compare.
    /// </param>
    /// <param name="key2">
    /// A <see cref="T:System.Data.Entity.Core.EntityKey" /> to compare.
    /// </param>
    public static bool operator ==(EntityKey key1, EntityKey key2)
    {
      return EntityKey.InternalEquals(key1, key2, true);
    }

    /// <summary>
    /// Compares two <see cref="T:System.Data.Entity.Core.EntityKey" /> objects.
    /// </summary>
    /// <returns>true if the  key1  and  key2  values are not equal; otherwise, false.</returns>
    /// <param name="key1">
    /// A <see cref="T:System.Data.Entity.Core.EntityKey" /> to compare.
    /// </param>
    /// <param name="key2">
    /// A <see cref="T:System.Data.Entity.Core.EntityKey" /> to compare.
    /// </param>
    public static bool operator !=(EntityKey key1, EntityKey key2)
    {
      return !EntityKey.InternalEquals(key1, key2, true);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    internal static bool InternalEquals(EntityKey key1, EntityKey key2, bool compareEntitySets)
    {
      if (object.ReferenceEquals((object) key1, (object) key2))
        return true;
      if (object.ReferenceEquals((object) key1, (object) null) || object.ReferenceEquals((object) key2, (object) null) || (object.ReferenceEquals((object) EntityKey.NoEntitySetKey, (object) key1) || object.ReferenceEquals((object) EntityKey.EntityNotValidKey, (object) key1)) || (object.ReferenceEquals((object) EntityKey.NoEntitySetKey, (object) key2) || object.ReferenceEquals((object) EntityKey.EntityNotValidKey, (object) key2) || key1.GetHashCode() != key2.GetHashCode() && compareEntitySets) || key1._containsByteArray != key2._containsByteArray)
        return false;
      if (key1._singletonKeyValue != null)
      {
        if (key1._containsByteArray)
        {
          if (key2._singletonKeyValue == null || !ByValueEqualityComparer.CompareBinaryValues((byte[]) key1._singletonKeyValue, (byte[]) key2._singletonKeyValue))
            return false;
        }
        else if (!key1._singletonKeyValue.Equals(key2._singletonKeyValue))
          return false;
        if (!string.Equals(key1._keyNames[0], key2._keyNames[0]))
          return false;
      }
      else
      {
        if (key1._compositeKeyValues == null || key2._compositeKeyValues == null || key1._compositeKeyValues.Length != key2._compositeKeyValues.Length)
          return false;
        if (key1._containsByteArray)
        {
          if (!EntityKey.CompositeValuesWithBinaryEqual(key1, key2))
            return false;
        }
        else if (!EntityKey.CompositeValuesEqual(key1, key2))
          return false;
      }
      return !compareEntitySets || string.Equals(key1._entitySetName, key2._entitySetName) && string.Equals(key1._entityContainerName, key2._entityContainerName);
    }

    internal static bool CompositeValuesWithBinaryEqual(EntityKey key1, EntityKey key2)
    {
      for (int index = 0; index < key1._compositeKeyValues.Length; ++index)
      {
        if (key1._keyNames[index].Equals(key2._keyNames[index]))
        {
          if (!ByValueEqualityComparer.Default.Equals(key1._compositeKeyValues[index], key2._compositeKeyValues[index]))
            return false;
        }
        else if (!EntityKey.ValuesWithBinaryEqual(key1._keyNames[index], key1._compositeKeyValues[index], key2))
          return false;
      }
      return true;
    }

    private static bool ValuesWithBinaryEqual(string keyName, object keyValue, EntityKey key2)
    {
      for (int index = 0; index < key2._keyNames.Length; ++index)
      {
        if (string.Equals(keyName, key2._keyNames[index]))
          return ByValueEqualityComparer.Default.Equals(keyValue, key2._compositeKeyValues[index]);
      }
      return false;
    }

    private static bool CompositeValuesEqual(EntityKey key1, EntityKey key2)
    {
      for (int index = 0; index < key1._compositeKeyValues.Length; ++index)
      {
        if (key1._keyNames[index].Equals(key2._keyNames[index]))
        {
          if (!object.Equals(key1._compositeKeyValues[index], key2._compositeKeyValues[index]))
            return false;
        }
        else if (!EntityKey.ValuesEqual(key1._keyNames[index], key1._compositeKeyValues[index], key2))
          return false;
      }
      return true;
    }

    private static bool ValuesEqual(string keyName, object keyValue, EntityKey key2)
    {
      for (int index = 0; index < key2._keyNames.Length; ++index)
      {
        if (string.Equals(keyName, key2._keyNames[index]))
          return object.Equals(keyValue, key2._compositeKeyValues[index]);
      }
      return false;
    }

    internal KeyValuePair<string, DbExpression>[] GetKeyValueExpressions(
      EntitySet entitySet)
    {
      int num = 0;
      if (!this.IsTemporary)
        num = this._singletonKeyValue == null ? this._compositeKeyValues.Length : 1;
      if (((EntitySetBase) entitySet).ElementType.KeyMembers.Count != num)
        throw new ArgumentException(Strings.EntityKey_EntitySetDoesNotMatch((object) TypeHelpers.GetFullName(entitySet.EntityContainer.Name, entitySet.Name)), nameof (entitySet));
      KeyValuePair<string, DbExpression>[] keyValuePairArray;
      if (this._singletonKeyValue != null)
      {
        EdmMember keyMember = ((EntitySetBase) entitySet).ElementType.KeyMembers[0];
        keyValuePairArray = new KeyValuePair<string, DbExpression>[1]
        {
          Helper.GetModelTypeUsage(keyMember).Constant(this._singletonKeyValue).As(keyMember.Name)
        };
      }
      else
      {
        keyValuePairArray = new KeyValuePair<string, DbExpression>[this._compositeKeyValues.Length];
        for (int index = 0; index < this._compositeKeyValues.Length; ++index)
        {
          EdmMember keyMember = ((EntitySetBase) entitySet).ElementType.KeyMembers[index];
          keyValuePairArray[index] = Helper.GetModelTypeUsage(keyMember).Constant(this._compositeKeyValues[index]).As(keyMember.Name);
        }
      }
      return keyValuePairArray;
    }

    internal string ConcatKeyValue()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("EntitySet=").Append(this._entitySetName);
      if (!this.IsTemporary)
      {
        foreach (EntityKeyMember entityKeyValue in this.EntityKeyValues)
        {
          stringBuilder.Append(';');
          stringBuilder.Append(entityKeyValue.Key).Append("=").Append(entityKeyValue.Value);
        }
      }
      return stringBuilder.ToString();
    }

    internal object FindValueByName(string keyName)
    {
      if (this.SingletonKeyValue != null)
        return this._singletonKeyValue;
      object[] compositeKeyValues = this.CompositeKeyValues;
      for (int index = 0; index < compositeKeyValues.Length; ++index)
      {
        if (keyName == this._keyNames[index])
          return compositeKeyValues[index];
      }
      throw new ArgumentOutOfRangeException(nameof (keyName));
    }

    internal void InitializeEntitySetName(string qualifiedEntitySetName)
    {
      string[] strArray = qualifiedEntitySetName.Split('.');
      if (strArray.Length != 2 || string.IsNullOrWhiteSpace(strArray[0]) || string.IsNullOrWhiteSpace(strArray[1]))
        throw new ArgumentException(Strings.EntityKey_InvalidQualifiedEntitySetName, nameof (qualifiedEntitySetName));
      this._entityContainerName = strArray[0];
      this._entitySetName = strArray[1];
      EntityKey.ValidateName(this._entityContainerName);
      EntityKey.ValidateName(this._entitySetName);
    }

    private static void ValidateName(string name)
    {
      if (!name.IsValidUndottedName())
        throw new ArgumentException(Strings.EntityKey_InvalidName((object) name));
    }

    internal bool InitializeKeyValues(
      IEnumerable<KeyValuePair<string, object>> entityKeyValues,
      bool allowNullKeys = false,
      bool tokenizeStrings = false)
    {
      int length = entityKeyValues.Count<KeyValuePair<string, object>>();
      if (length == 1)
      {
        this._keyNames = new string[1];
        KeyValuePair<string, object> keyValuePair = entityKeyValues.Single<KeyValuePair<string, object>>();
        this.InitializeKeyValue(keyValuePair, 0, tokenizeStrings);
        this._singletonKeyValue = keyValuePair.Value;
      }
      else if (length > 1)
      {
        this._keyNames = new string[length];
        this._compositeKeyValues = new object[length];
        int i = 0;
        foreach (KeyValuePair<string, object> entityKeyValue in entityKeyValues)
        {
          this.InitializeKeyValue(entityKeyValue, i, tokenizeStrings);
          this._compositeKeyValues[i] = entityKeyValue.Value;
          ++i;
        }
      }
      else if (!allowNullKeys)
        throw new ArgumentException(Strings.EntityKey_EntityKeyMustHaveValues, nameof (entityKeyValues));
      return length > 0;
    }

    [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
    private void InitializeKeyValue(
      KeyValuePair<string, object> keyValuePair,
      int i,
      bool tokenizeStrings)
    {
      if (EntityUtil.IsNull(keyValuePair.Value) || string.IsNullOrWhiteSpace(keyValuePair.Key))
        throw new ArgumentException(Strings.EntityKey_NoNullsAllowedInKeyValuePairs, "entityKeyValues");
      EntityKey.ValidateName(keyValuePair.Key);
      this._keyNames[i] = tokenizeStrings ? EntityKey.LookupSingletonName(keyValuePair.Key) : keyValuePair.Key;
    }

    private void InitializeKeyValues(EntitySet entitySet, IExtendedDataRecord record)
    {
      int count = entitySet.ElementType.KeyMembers.Count;
      this._keyNames = entitySet.ElementType.KeyMemberNames;
      EntityType edmType = (EntityType) record.DataRecordInfo.RecordType.EdmType;
      if (count == 1)
      {
        this._singletonKeyValue = record[edmType.KeyMembers[0].Name];
        if (EntityUtil.IsNull(this._singletonKeyValue))
          throw new ArgumentException(Strings.EntityKey_NoNullsAllowedInKeyValuePairs, nameof (record));
      }
      else
      {
        this._compositeKeyValues = new object[count];
        for (int index = 0; index < count; ++index)
        {
          this._compositeKeyValues[index] = record[edmType.KeyMembers[index].Name];
          if (EntityUtil.IsNull(this._compositeKeyValues[index]))
            throw new ArgumentException(Strings.EntityKey_NoNullsAllowedInKeyValuePairs, nameof (record));
        }
      }
    }

    internal void ValidateEntityKey(MetadataWorkspace workspace, EntitySet entitySet)
    {
      this.ValidateEntityKey(workspace, entitySet, false, (string) null);
    }

    internal void ValidateEntityKey(
      MetadataWorkspace workspace,
      EntitySet entitySet,
      bool isArgumentException,
      string argumentName)
    {
      if (entitySet == null)
        return;
      ReadOnlyMetadataCollection<EdmMember> keyMembers = ((EntitySetBase) entitySet).ElementType.KeyMembers;
      if (this._singletonKeyValue != null)
      {
        if (keyMembers.Count != 1)
        {
          if (isArgumentException)
            throw new ArgumentException(Strings.EntityKey_IncorrectNumberOfKeyValuePairs((object) entitySet.ElementType.FullName, (object) keyMembers.Count, (object) 1), argumentName);
          throw new InvalidOperationException(Strings.EntityKey_IncorrectNumberOfKeyValuePairs((object) entitySet.ElementType.FullName, (object) keyMembers.Count, (object) 1));
        }
        EntityKey.ValidateTypeOfKeyValue(workspace, keyMembers[0], this._singletonKeyValue, isArgumentException, argumentName);
        if (!(this._keyNames[0] != keyMembers[0].Name))
          return;
        if (isArgumentException)
          throw new ArgumentException(Strings.EntityKey_MissingKeyValue((object) keyMembers[0].Name, (object) entitySet.ElementType.FullName), argumentName);
        throw new InvalidOperationException(Strings.EntityKey_MissingKeyValue((object) keyMembers[0].Name, (object) entitySet.ElementType.FullName));
      }
      if (this._compositeKeyValues == null)
        return;
      if (keyMembers.Count != this._compositeKeyValues.Length)
      {
        if (isArgumentException)
          throw new ArgumentException(Strings.EntityKey_IncorrectNumberOfKeyValuePairs((object) entitySet.ElementType.FullName, (object) keyMembers.Count, (object) this._compositeKeyValues.Length), argumentName);
        throw new InvalidOperationException(Strings.EntityKey_IncorrectNumberOfKeyValuePairs((object) entitySet.ElementType.FullName, (object) keyMembers.Count, (object) this._compositeKeyValues.Length));
      }
      for (int index1 = 0; index1 < this._compositeKeyValues.Length; ++index1)
      {
        EdmMember keyMember = ((EntitySetBase) entitySet).ElementType.KeyMembers[index1];
        bool flag = false;
        for (int index2 = 0; index2 < this._compositeKeyValues.Length; ++index2)
        {
          if (keyMember.Name == this._keyNames[index2])
          {
            EntityKey.ValidateTypeOfKeyValue(workspace, keyMember, this._compositeKeyValues[index2], isArgumentException, argumentName);
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          if (isArgumentException)
            throw new ArgumentException(Strings.EntityKey_MissingKeyValue((object) keyMember.Name, (object) entitySet.ElementType.FullName), argumentName);
          throw new InvalidOperationException(Strings.EntityKey_MissingKeyValue((object) keyMember.Name, (object) entitySet.ElementType.FullName));
        }
      }
    }

    private static void ValidateTypeOfKeyValue(
      MetadataWorkspace workspace,
      EdmMember keyMember,
      object keyValue,
      bool isArgumentException,
      string argumentName)
    {
      EdmType edmType = keyMember.TypeUsage.EdmType;
      if (Helper.IsPrimitiveType(edmType))
      {
        Type clrEquivalentType = ((PrimitiveType) edmType).ClrEquivalentType;
        if (!(clrEquivalentType != keyValue.GetType()))
          return;
        if (isArgumentException)
          throw new ArgumentException(Strings.EntityKey_IncorrectValueType((object) keyMember.Name, (object) clrEquivalentType.FullName, (object) keyValue.GetType().FullName), argumentName);
        throw new InvalidOperationException(Strings.EntityKey_IncorrectValueType((object) keyMember.Name, (object) clrEquivalentType.FullName, (object) keyValue.GetType().FullName));
      }
      EnumType objectSpaceType;
      if (workspace.TryGetObjectSpaceType((EnumType) edmType, out objectSpaceType))
      {
        Type clrType = objectSpaceType.ClrType;
        if (!(clrType != keyValue.GetType()))
          return;
        if (isArgumentException)
          throw new ArgumentException(Strings.EntityKey_IncorrectValueType((object) keyMember.Name, (object) clrType.FullName, (object) keyValue.GetType().FullName), argumentName);
        throw new InvalidOperationException(Strings.EntityKey_IncorrectValueType((object) keyMember.Name, (object) clrType.FullName, (object) keyValue.GetType().FullName));
      }
      if (isArgumentException)
        throw new ArgumentException(Strings.EntityKey_NoCorrespondingOSpaceTypeForEnumKeyMember((object) keyMember.Name, (object) edmType.FullName), argumentName);
      throw new InvalidOperationException(Strings.EntityKey_NoCorrespondingOSpaceTypeForEnumKeyMember((object) keyMember.Name, (object) edmType.FullName));
    }

    [Conditional("DEBUG")]
    private void AssertCorrectState(EntitySetBase entitySet, bool isTemporary)
    {
      if (this._singletonKeyValue != null)
      {
        if (entitySet == null)
          ;
      }
      else if (this._compositeKeyValues != null)
      {
        int num = 0;
        while (num < this._compositeKeyValues.Length)
          ++num;
      }
      else
      {
        int num1 = this.IsTemporary ? 1 : 0;
      }
    }

    /// <summary>
    /// Helper method that is used to deserialize an <see cref="T:System.Data.Entity.Core.EntityKey" />.
    /// </summary>
    /// <param name="context">Describes the source and destination of a given serialized stream, and provides an additional caller-defined context.</param>
    [SuppressMessage("Microsoft.Usage", "CA2238:ImplementSerializationMethodsCorrectly")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [OnDeserializing]
    public void OnDeserializing(StreamingContext context)
    {
      if (!this.RequiresDeserialization)
        return;
      this.DeserializeMembers();
    }

    /// <summary>
    /// Helper method that is used to deserialize an <see cref="T:System.Data.Entity.Core.EntityKey" />.
    /// </summary>
    /// <param name="context">Describes the source and destination of a given serialized stream and provides an additional caller-defined context.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Microsoft.Usage", "CA2238:ImplementSerializationMethodsCorrectly")]
    [Browsable(false)]
    [OnDeserialized]
    public void OnDeserialized(StreamingContext context)
    {
      this._entitySetName = EntityKey.LookupSingletonName(this._entitySetName);
      this._entityContainerName = EntityKey.LookupSingletonName(this._entityContainerName);
      if (this._keyNames == null)
        return;
      for (int index = 0; index < this._keyNames.Length; ++index)
        this._keyNames[index] = EntityKey.LookupSingletonName(this._keyNames[index]);
    }

    internal static string LookupSingletonName(string name)
    {
      if (!string.IsNullOrEmpty(name))
        return EntityKey.NameLookup.GetOrAdd(name, (Func<string, string>) (n => n));
      return (string) null;
    }

    private void ValidateWritable(object instance)
    {
      if (this._isLocked || instance != null)
        throw new InvalidOperationException(Strings.EntityKey_CannotChangeKey);
    }

    private bool RequiresDeserialization
    {
      get
      {
        return this._deserializedMembers != null;
      }
    }

    private void DeserializeMembers()
    {
      if (!this.InitializeKeyValues((IEnumerable<KeyValuePair<string, object>>) new EntityKey.KeyValueReader((IEnumerable<EntityKeyMember>) this._deserializedMembers), true, true))
        return;
      this._deserializedMembers = (EntityKeyMember[]) null;
    }

    private class KeyValueReader : IEnumerable<KeyValuePair<string, object>>, IEnumerable
    {
      private readonly IEnumerable<EntityKeyMember> _enumerator;

      public KeyValueReader(IEnumerable<EntityKeyMember> enumerator)
      {
        this._enumerator = enumerator;
      }

      public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
      {
        foreach (EntityKeyMember entityKeyMember in this._enumerator)
        {
          if (entityKeyMember != null)
            yield return new KeyValuePair<string, object>(entityKeyMember.Key, entityKeyMember.Value);
        }
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }
    }
  }
}
