using Blazor_WASM_gRPC_EFCore.Server.Data;
using Blazor_WASM_gRPC_EFCore.Shared.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Blazor_WASM_gRPC_EFCore.Server.Services
{
	public class BlogService : BlogProto.BlogProtoBase
	{
		private readonly ApplicationDbContext dbContext;
		public BlogService(ApplicationDbContext dataContext)
		{
			dbContext = dataContext;
		}

		public override async Task<Posts> GetPosts(Empty request, ServerCallContext context)
		{
			List<Post> postsQuery = await dbContext.Posts!
				.Include(tipd => tipd.TagsInPostRepeated)
				.OrderByDescending(dc => dc.CreatedUtcTicks)
				.AsNoTracking()
				.ToListAsync();

			// The Protobuf serializer doesn't support reference loops
			// see: https://github.com/grpc/grpc-dotnet/issues/1177#issuecomment-763910215
			//var posts = new Posts();
			//posts.PostsData.AddRange(postsQuery); // so this doesn't work

			Posts posts = new();
			foreach (var p in postsQuery)
			{
				Post post = new()
				{
					PostId = p.PostId,
					Title = p.Title,
					Content = p.Content,
					CreatedUtcTicks = p.CreatedUtcTicks,
				};

				// Just add all the tags to each post, this isn't a reference loop (many to many posts/tags).
				List<Tag> tags = p.TagsInPostRepeated.Select(t => new Tag { TagId = t.TagId }).ToList();
				post.TagsInPostRepeated.AddRange(tags);

				// Add post (now with tags) to posts
				posts.PostsRepeated.Add(post);
			}
			return posts;
		}

		public override async Task<AddPostResult> AddPost(Post postRequest, ServerCallContext context)
		{
			AddPostResult addPostResult = new();

			// postRequest console output
			Console.WriteLine("\n\n***** Post");
			Console.WriteLine($"***** postRequest.Title: {postRequest.Title}");
			Console.WriteLine($"***** postRequest.Content: {postRequest.Content}");
			Console.WriteLine("\n***** Tags in Post");
			foreach (Tag tag in postRequest.TagsInPostRepeated)
			{
				Console.WriteLine($"***** tag.TagId: {tag.TagId}");
			}
			Console.WriteLine("\n");

			// no error checking, sample.
			dbContext.Posts!.Add(postRequest);
			await dbContext.SaveChangesAsync();

			addPostResult.PostId = postRequest.PostId; // record id
			return addPostResult;
		}
	}
}
