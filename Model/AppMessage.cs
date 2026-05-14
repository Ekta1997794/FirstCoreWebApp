namespace FirstCoreWebApp.Model
{
    public static class AppMessages
    {
            public const string InvalidEmail = "Invalid email format";
            public const string AlreadyExists = "Already exists";
            public const string IncorrectFormat = "Password format is incorrect";
            public const string CreateSuccess = "Successfully created";
            public const string InvalidCred = "Invalid credentials";
            public const string Required = "Required field";
            public const string RoleExists = "Role already exists";
            public const string RoleCreated = "Role created successfully";
            public const string LoginSuccess = "Login successful";
            public const string LogoutSuccess = "Logout successful";
            public const string TokenGenerated = "New access token generated";
            public const string ProfileFetched = "Profile fetched successfully";
            public const string AdminWelcome = "Welcome Admin";
            public const string InvalidRefreshToken = "Invalid refresh token";
            public const string RefreshTokenRevoked = "Refresh token has been revoked";
            public const string RefreshTokenExpired = "Refresh token expired, please login again";
            public const string RolesFound = "All roles are available";
            public const string NotFound = "Not Found";
            public const string InvalidRole = "Invalid Role";
            public const string RoleAssign = "Role Assigned";
            public const string RoleFetched = "Role Fetched";
            public const string InvalidStatus = "Invalid Status Value";
            public const string CourseUpdate = "Course Update Successfully";
        public const string PrerequisiteCourseIds = "Course ID {0} does not exist";
        
        public const string EditNotAllowed = "You are not allowed to edit this course";
        public const string DeleteNotAllowed = "You are not allowed to delete this course";
        public const string PublishNotAllowed = "You are not allowed to publish this course";
        public const string PublishSuccess =    "Course published successfully";
        public const string CheckModuleContainCourseId = "Course must contain at least one module before publishing";
        public const string CoureseDeleted = "Course deleted successfully";
        
    }
}
