﻿using System;
using Known.Data;

namespace Known
{
    public abstract class ServiceBase
    {
        public ServiceBase(Context context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Context Context { get; }

        protected T LoadBusiness<T>() where T : ServiceBase
        {
            return ObjectFactory.CreateService<T>(Context);
        }

        protected T LoadRepository<T>() where T : IRepository
        {
            return ObjectFactory.CreateRepository<T>(Context);
        }
    }

    public abstract class ServiceBase<TRepository> : ServiceBase
        where TRepository : IRepository
    {
        public ServiceBase(Context context) : base(context)
        {
        }

        public TRepository Repository
        {
            get { return LoadRepository<TRepository>(); }
        }
    }
}
