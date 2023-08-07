﻿using System;
using System.Collections.Generic;

namespace LuvFinder.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime Date { get; set; }

    public virtual ICollection<UserLike> UserLikeFroms { get; set; } = new List<UserLike>();

    public virtual ICollection<UserLike> UserLikeTos { get; set; } = new List<UserLike>();

    public virtual ICollection<UserMessage> UserMessageFroms { get; set; } = new List<UserMessage>();

    public virtual ICollection<UserMessage> UserMessageTos { get; set; } = new List<UserMessage>();

    public virtual ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();

    public virtual ICollection<UserViewing> UserViewingUserVieweds { get; set; } = new List<UserViewing>();

    public virtual ICollection<UserViewing> UserViewingUserViewingNavigations { get; set; } = new List<UserViewing>();
}
