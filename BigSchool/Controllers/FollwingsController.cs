using BigSchool.DTOs;
using BigSchool.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BigSchool.Controllers
{
    public class FollwingsController : ApiController
    {
        private readonly ApplicationDbContext _dbContext;
        public FollwingsController()
        {
            _dbContext = new ApplicationDbContext();
        }
        [HttpPost]
        public IHttpActionResult Follow(FollowingDto followingDTO)
        {
            var userID = User.Identity.GetUserId();
            if (_dbContext.Followings.Any(f => f.FollowerId == userID && f.FolloweeId == followingDTO.FolloweeId))
            {
                return BadRequest("The Attendance already exists");
            }
            var following = new Following
            {
                FollowerId = userID,
                FolloweeId = followingDTO.FolloweeId
            };
            _dbContext.Followings.Add(following);
            _dbContext.SaveChanges();
            return Ok();
        }
    }
}
