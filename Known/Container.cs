﻿using System;
using System.Collections;
using System.Reflection;

namespace Known
{
    /// <summary>
    /// 应用程序对象容器类。
    /// </summary>
    public sealed class Container
    {
        private static readonly Hashtable cached = new Hashtable();

        /// <summary>
        /// 清除所有缓存的对象实例。
        /// </summary>
        public static void Clear()
        {
            cached.Clear();
        }

        public static T Resolve<T>()
        {
            var key = typeof(T);
            if (cached.ContainsKey(key))
                return (T)cached[key];

            var key1 = typeof(T).Name;
            if (cached.ContainsKey(key1))
                return (T)cached[key1];

            return default;
        }

        public static object Resolve(string name)
        {
            if (!cached.ContainsKey(name))
                return null;

            return cached[name];
        }

        public static void Register<T, TImpl>() where TImpl : T
        {
            var key = typeof(T);
            if (!cached.ContainsKey(key))
            {
                lock (cached.SyncRoot)
                {
                    if (!cached.ContainsKey(key))
                    {
                        cached[key] = Activator.CreateInstance<TImpl>();
                    }
                }
            }
        }

        public static void Register<T>(object instance)
        {
            var key = typeof(T);
            Register(key, instance);
        }

        public static void Register<T>(Assembly assembly, params object[] args)
        {
            if (assembly == null)
                return;

            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsSubclassOf(typeof(T)) && !type.IsAbstract)
                {
                    var instance = args != null && args.Length > 0
                                 ? Activator.CreateInstance(type, args)
                                 : Activator.CreateInstance(type);
                    Register(type.Name, instance);
                }
            }
        }

        private static void Register(object key, object instance)
        {
            if (!cached.ContainsKey(key))
            {
                lock (cached.SyncRoot)
                {
                    if (!cached.ContainsKey(key))
                    {
                        cached[key] = instance;
                    }
                }
            }
        }
    }
}
