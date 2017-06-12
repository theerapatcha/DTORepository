﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using DTORepositoryTest.Samples.Models;
using System.Linq;
using System.Data.Entity;
using Moq;
using System.IO;
using Effort.DataLoaders;
using System.Data.Common;
using DTORepository;

namespace DTORepositoryTest
{
    public class UnitTestBase : IDisposable
    {
        protected BloggingContext _context;
        public BloggingContext PrepareContext()
        {
            DbConnection connection =
            Effort.DbConnectionFactory.CreateTransient();

            BloggingContext context = new BloggingContext(connection);
            context.Blogs.Add(new Blog
            {
                BlogId = 1,
                Name = "Dummy Blog #1",
                Url = "http://usc.edu",
                Author = new Author { Name="John Doe" },
                Posts = new List<Post> {
                    new Post { PostId = 1, Title = "T1", Content = "C1" },
                    new Post { PostId = 2, Title = "T2", Content = "C1" }
                },
                Tags = new List<BlogTag> { new BlogTag { Name = "test" }}
            });
            context.Blogs.Add(new Blog
            {
                BlogId = 2,
                Name = "Dummy Blog #2",
                Url = "http://google.com",
                Author = new Author { Name = "Jame Harden" },
                Posts = new List<Post> {
                    new Post { PostId = 3, Title = "T3", Content = "C2" },
                    new Post { PostId = 4, Title = "T4", Content = "C2" }
                }
            });
            context.SaveChanges();
            return context;
        }
        public UnitTestBase()
        {
            this._context = PrepareContext();
            
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    this._context.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UnitTestBase() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
