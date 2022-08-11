using Blazor_WASM_gRPC_EFCore.Shared.Protos;
using Microsoft.EntityFrameworkCore;

namespace Blazor_WASM_gRPC_EFCore.Server.Data
{
	public static class SeedData
	{
		public static void Initialize(IServiceProvider serviceProvider)
		{
			using (var ctx = new ApplicationDbContext(
				serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
			{
				// Look for any posts.
				if (ctx.Posts is not null)
				{
					if (ctx.Posts.Any())
						return;   // DB has been seeded
				}
				else
					throw new Exception("ctx.Posts is null");

				long utcNowTicks = DateTime.UtcNow.Ticks;

				// Post Records
				Post post1 = new Post { PostId = 1, Title = "Post Title 1", Content = "Post Content I", CreatedUtcTicks = utcNowTicks };
				Post post2 = new Post { PostId = 2, Title = "Post Title 2", Content = "Post Content II", CreatedUtcTicks = utcNowTicks };
				Post post3 = new Post { PostId = 3, Title = "Post Title 3", Content = "Post Content III", CreatedUtcTicks = utcNowTicks };
				Post post4 = new Post { PostId = 4, Title = "Post Title 4", Content = "Post Content IV", CreatedUtcTicks = utcNowTicks };

				// Tag Records
				Tag tag1 = new Tag { TagId = "Tag One" };
				Tag tag2 = new Tag { TagId = "Tag Two" };
				Tag tag3 = new Tag { TagId = "Tag Three" };
				Tag tag4 = new Tag { TagId = "Tag Four" };
				Tag tag5 = new Tag { TagId = "Tag Five" };
				Tag tag6 = new Tag { TagId = "Tag Six" };

				// Add Multiple Tags to Posts, many to many
				post1.TagsInPostRepeated.AddRange(new[] { tag1, tag2, tag3, tag4, tag5, tag6 });
				post2.TagsInPostRepeated.AddRange(new[] { tag3, tag4, tag6 });
				post3.TagsInPostRepeated.AddRange(new[] { tag4 });
				// post 4 without tags

				// Add records to DB
				ctx.AddRange(post1, post2, post3, post4);
				ctx.AddRange(tag1, tag2, tag3, tag4, tag5, tag6);

				ctx.SaveChanges();
			}
		}
	}
}
