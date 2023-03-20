using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    // our userAddress has got an Id field that is an integer; however, while our user entity has an ID property that is a string. It's not going to work when it comes to specify what the foreigh key is for our address because they need to match the types. If we're going to have a primary key as a string in one entity, then we need a primary key that's a string in the other entity. The easiest way to go about this is just to make it so that we use ints everywhere rather than having some entitites that use strings and some entities that use ints.
    // we use <int> to override te IdentityUser default string. When we do this because we're also using roles in our applicaton, then our roles are also going to need to use an integer. In order to make the role use an integer, we're going to need to create another class that derives from identity role and override the default string with int.
    public class User : IdentityUser<int>
    {
        // we don't have to specify anything here, as everything we need is covered by IdentityUser
        public UserAddress Address {get; set;}

        
    }
}