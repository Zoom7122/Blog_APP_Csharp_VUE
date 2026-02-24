using blogApp_DAL;
using blogApp_DAL.Model;
using BlogAPP_DAL.Intarface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace BlogAPP_DAL.Repository
{
    public class TagRepo : ITagRepo
    {

        private readonly Blog_DBcontext _context;

        public TagRepo(Blog_DBcontext context)
        {
            _context = context;
        }

        public async Task CreateTagAsync(Tag tag)
        {
            var entry = _context.Entry(tag);
            if (entry.State == EntityState.Detached)
                await _context.Tags.AddAsync(tag);

            await _context.SaveChangesAsync();
        }

        public async Task<Tag> FindTagByName(string name)
        {
            return await _context.Tags.FirstOrDefaultAsync(x => x.Name == name);
        }


    }
}
