/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Piranha.AspNetCore.Identity
{
    public class IdentitySecurity : ISecurity
    {
        /// <summary>
        /// The sign in manager.
        /// </summary>
        private readonly SignInManager<Data.User> _signInManager;

        /// <summary>
        /// The optional identity seed.
        /// </summary>
        private readonly IIdentitySeed _seed;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public IdentitySecurity(SignInManager<Data.User> signInManager, IIdentitySeed seed = null) 
        {
            _signInManager = signInManager;
            _seed = seed;
        }

        /// <summary>
        /// Authenticates and signs in the user with the
        /// given credentials.
        /// </summary>
        /// <param name="context">The current application context</param>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>If the user was signed in</returns>
        public async Task<bool> SignIn(object context, string username, string password) {
            if (_seed != null)
                await _seed.CreateAsync();

            var result = await _signInManager.PasswordSignInAsync(username, password, false, lockoutOnFailure: false);
            return result.Succeeded;
        }

        /// <summary>
        /// Signs out the current user.
        /// </summary>
        /// <param name="context">The current application context</param>
        public Task SignOut(object context) {
            return _signInManager.SignOutAsync();
        }        
    }
}