// --------------------------------------------
//  Project: Network Chat App
//  Engineer: Ivana Bavin-Gomez-San Basilio
//  Date: May 10th 2026
//  Description: User Status Entity
// --------------------------------------------

using Backend.API.src.Core.Enums;

namespace Backend.API.src.Core.Entities
{
    public class UserStatus
    {

        //----------------------------------
        //-------  Private Fields ----------
        //----------------------------------

        //-----Main class attributes
        private Guid _userId;

        // Statuses: Inactive = 0, Active = 1, Custom = 2
        private UserStateType _state = UserStateType.Inactive; // The default status is inactive (0)
        private string? _customText = string.Empty;




        //----------------------------------
        //------  Getters and Setters ------
        //----------------------------------

        //-----Main class attributes



        public Guid UserId
        {
            get => _userId;
            set => _userId = value;
        }


        public UserStateType State
        {
            get => _state;
            set => _state = value;
        }

        public string? CustomText
        {
            get => _customText;
            set => _customText = value;
        }


    }
}
