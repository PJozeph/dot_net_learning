using dot_net_api.Data;
using dot_net_learning_api.DTO;
using dot_net_learning_api.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dot_net_learning_api.controllers
{


    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dataContextDapper;
        private readonly IConfiguration _configuration;

        public PostController(IConfiguration configuration)
        {
            _configuration = configuration;
            _dataContextDapper = new DataContextDapper(_configuration);
        }

        


        [HttpGet("GetUserPostsWithToken")]
        public IActionResult getUserPostWithToken()
        {
            string userId = User.FindFirst("userId")?.Value + "";
            IEnumerable<Post> postList = _dataContextDapper.LoadData<Post>($"SELECT * FROM DotNetCourseDatabase.TutorialAppSchema.Posts WHERE UserId = '{userId}'");
            if (postList == null)
            {
                return NotFound();
            }
            return Ok(postList);
        }

        [HttpPost("AddPost")]
        public IActionResult addPost(PostToAddDTO postToAddDTO)
        {
            string userId = User.FindFirst("userId")?.Value + "";
            string sqlTimeNow = "GETDATE()";
            string sqlCreatePost = $"INSERT INTO DotNetCourseDatabase.TutorialAppSchema.Posts (UserId, PostTitle, PostContent, PostCreated, PostUpdated) VALUES ('{userId}', '{postToAddDTO.PostTitle}', '{postToAddDTO.PostContent}', {sqlTimeNow}, {sqlTimeNow})";

            if (_dataContextDapper.SaveData<Post>(sqlCreatePost))
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPut("UpdatePost")]
        public IActionResult updatePost(PostToEDitDTO postToEDitDTO){
            string sqlTimeNow = "GETDATE()";
            string sqlUpdatePost = $"UPDATE DotNetCourseDatabase.TutorialAppSchema.Posts SET PostTitle = '{postToEDitDTO.PostTitle}', PostContent = '{postToEDitDTO.PostContent}', PostUpdated = {sqlTimeNow} WHERE PostId = {postToEDitDTO.PostId} AND UserId = {User.FindFirst("userId")?.Value}";

            if (_dataContextDapper.SaveData<Post>(sqlUpdatePost))
            {
                return Ok();
            }

            return BadRequest();            

        }

    }
}