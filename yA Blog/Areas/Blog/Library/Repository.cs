using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using yA_Blog.Areas.Blog.Models.Managers;

namespace yA_Blog.Areas.Blog.Library
{
    public class Repository<T> where T : class
    {
        private readonly DatabaseContext _db = new DatabaseContext();
        private DbSet<T> _objectSet;

        public Repository()
        {
            
            _objectSet = _db.Set<T>();
        }

        public List<T> SayfalariGetir(int? page, Expression<Func<T, int>> where)
        {
            if (page == null)
            {
                return null;
            }

            if (page >= 1)
            {
                var total = _objectSet.Select(where).Count();
                int sayfaSayisi = (total / 10) + 1;

                if (sayfaSayisi < page)
                {
                    return null;
                }
                else
                {
                    int skip = (int)(page - 1) * 10;

                    var result = Listele(where, skip);
                    return result;

                }
            }
            else
            {
                return null;
            }
        }

        public List<T> Listele(Expression<Func<T, int>> where,int skip)
        {
            return ListQueryable().OrderBy(where).
                Skip(skip).
                Take(10).
                ToList();
        }

        public int IcerikSayisi(Expression<Func<T, int>> where)
        {
            return _objectSet.Select(where).Count();
        }

        public IQueryable<T> ListQueryable()
        {
            return _objectSet.AsQueryable<T>();
        }
    }
}