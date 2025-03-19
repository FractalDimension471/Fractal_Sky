using System;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE.LogicalLine
{
    public class VariableStore
    {
        #region 属性/Property
        private static string ID_DefaultDatabaseName { get; } = "Default";
        public static char ID_RelationalSeperator { get; } = '.';
        public static char ID_Variable { get; } = '$';
        public static string R_Variables { get; } = @"[!]?\$[a-zA-Z0-9_.]+";
        #endregion
        #region 方法/Method
        public class Database
        {
            public string DatabaseName { get; set; }
            public Dictionary<string, Variable> Variables { get; protected set; }
            public Database(string name)
            {
                DatabaseName = name;
                Variables = new();
            }
        }

        public abstract class Variable
        {
            public abstract object Get();
            public abstract void Set(object newValue);
        }
        public class Variable<T> : Variable
        {
            private T Value { get; }
            private Func<T> Getter { get; }
            private Action<T> Setter { get; }

            public Variable(T value = default, Func<T> getter = null, Action<T> setter = null)
            {
                this.Value = value;
                if (getter == null)
                {
                    this.Getter = () => value;
                }
                else
                {
                    this.Getter = getter;
                }

                if (setter == null)
                {
                    this.Setter = newValue => value = newValue;
                }
                else
                {
                    this.Setter = setter;
                }
            }

            public override object Get() => Getter();

            public override void Set(object newValue) => Setter((T)newValue);
        }
        public static Dictionary<string, Database> Databases { get; protected set; } = new() { { ID_DefaultDatabaseName, new(ID_DefaultDatabaseName) } };
        public static Database DefaultDatabase => Databases[ID_DefaultDatabaseName];
        public static bool TryCreateDatabase(string name)
        {
            if (!Databases.ContainsKey(name))
            {
                Databases[name] = new(name);
                return true;
            }
            return false;
        }
        public static Database GetDatabase(string name)
        {
            if (name == string.Empty)
            {
                return DefaultDatabase;
            }
            if (!Databases.ContainsKey(name))
            {
                if (!TryCreateDatabase(name))
                {
                    Debug.LogWarning($"Cannot create database '{name}'");
                    return null;
                }
            }
            return Databases[name];
        }
        private static (string[], Database, string) ExtractInfo(string name)
        {
            string[] parts = name.Split(ID_RelationalSeperator);
            Database database = GetDatabase(parts[0]);
            string variableName = parts.Length > 1 ? parts[1] : parts[0];
            return (parts, database, variableName);
        }
        public static bool TryCreateVariable<T>(string name, T defaultValue, Func<T> getter = null, Action<T> setter = null)
        {
            (_, Database database, string variableName) = ExtractInfo(name);

            if (database.Variables.ContainsKey(variableName))
            {
                return false;
            }
            database.Variables[variableName] = new Variable<T>(defaultValue, getter, setter);
            return true;
        }

        public static bool TryGetVariable(string name, out object value)
        {
            (_, Database database, string variableName) = ExtractInfo(name);
            if (!database.Variables.ContainsKey(variableName))
            {
                value = null;
                return false;
            }
            value = database.Variables[variableName].Get();
            return true;
        }
        public static bool TrySetVariable<T>(string name, T value, bool createIfNotExist = false)
        {
            (_, Database database, string variableName) = ExtractInfo(name);
            if (!database.Variables.ContainsKey(variableName))
            {
                if (createIfNotExist)
                {
                    TryCreateVariable(name, value);
                }
                return false;
            }

            //if(value is int)
            //{
            //    Debug.Log($"{value} is int");
            //}
            //else if (value is float)
            //{
            //    Debug.Log($"{value} is float");
            //}
            //else if(value is string)
            //{
            //    Debug.Log($"{value} is string");
            //}
            //else
            //{
            //    Debug.LogWarning($"{value} is {value.GetType().Name}");
            //}
            //以上代码用于测试"InvalidCastException: Specified cast is not valid"报错
            database.Variables[variableName].Set(value);
            return true;
        }
        public static bool HasVariable(string name)
        {
            string[] parts = name.Split(ID_RelationalSeperator);
            Database database = GetDatabase(parts[0]);
            string variableName = parts.Length > 1 ? parts[1] : parts[0];

            return database.Variables.ContainsKey(variableName);
        }
        public static void RemoveAllVariables()
        {
            Databases.Clear();
            Databases[ID_DefaultDatabaseName] = new(ID_DefaultDatabaseName);
        }
        public static void RemoveVariable(string name)
        {
            (_, Database database, string variableName) = ExtractInfo(name);
            if (database.Variables.ContainsKey(variableName))
            {
                database.Variables.Remove(variableName);
            }
        }
        public static void PrintAllDatabases()
        {
            foreach (var data in Databases)
            {
                Debug.Log($"Database:'{data}'");
            }
        }
        public static void PrintAllVariables(Database database = null)
        {
            if (database != null)
            {
                PrintingAllVariables(database);
                return;
            }
            foreach (var data in Databases)
            {
                PrintingAllVariables(data.Value);
            }
        }
        private static void PrintingAllVariables(Database database)
        {
            foreach (var data in database.Variables)
            {
                string variableName = data.Key;
                object variableValue = data.Value.Get();
                Debug.Log($"Database:'{database.DatabaseName}'|Variable'{variableName}'='{variableValue}'");
            }
        }
        #endregion
    }
}