//using Neo.Bpms.MetaModel.UserAndOrganization;
//using static Neo.Bpms.Domain.Model.UI.Form;

//namespace Hyper.AdminPanel.Web.ViewModels.Account;

//public class AssignAccessViewModel
//{
//    public List<GroupViewModel> Positions { get; set; }
//    public List<GroupViewModel> UserGroups { get; set; }
//    public List<AssignAccessItemViewModel> Items { get; set; }
//    public bool IsPrivatePage { get; set; }
//    public AssigneeType AssigneeType { get; set; }
//    public string AssigneeId { get; set; }
//}

//public class GroupViewModel
//{
//    public long Id;
//    public string Name;
//    public bool UserHasAccess;
//}
//public class AssignAccessItemViewModel
//{
//    public long? Id { get; set; }
//    public ESystemFeature SystemFeature { get; set; }
//    public string SystemFeatureAlias { get; set; }
//    public EActivityType ActivityType { get; set; }
//    public eFormType FormType { get; set; }
//    public string NamespaceId { get; set; }
//    public string NamespaceAlias { get; set; }
//    public string EntityId { get; set; }
//    public string EntityAlias { get; set; }
//    public string EntityItemId { get; set; } // Form/Report/Dashboard/Process Id
//    public string EntityItemAlias { get; set; }
//    public string FormSubjectId { get; set; }
//    public string FormSubjectAlias { get; set; }
//    public string Controller { get; set; }
//    public string Action { get; set; }
//    public bool IsDiactivated { get; set; }
//    public bool PrevIsDiactivated { get; set; }
//    public bool IsAccessible { get; set; }
//    public List<bool> UserGroupsIsAccessible { get; set; }

//    public bool EqualsIdentityUserActivity(ActivityAuthorization activityAuthorization)
//    {
//        return (long)ActivityType == activityAuthorization.ActivityTypeId &&
//               (long)FormType == activityAuthorization.ActivityFormTypeId &&
//               (NamespaceId ?? "") == (activityAuthorization.ActivityNamespaceId ?? "") &&
//               (EntityId ?? "") == (activityAuthorization.ActivityEntityId ?? "") &&
//               (EntityItemId ?? "") == (activityAuthorization.EntityItemId ?? "") &&
//               (Controller ?? "") == (activityAuthorization.ActivityControllerId ?? "") &&
//               (Action ?? "") == (activityAuthorization.ActivityControllerAction ?? "") &&
//               (long)SystemFeature == activityAuthorization.SystemFeatureId;
//    }

//    public bool EqualsWithoutItemIdCheck(ActivityAuthorization activityAuthorization)
//    {
//        return (long)ActivityType == activityAuthorization.ActivityTypeId &&
//               (long)FormType == activityAuthorization.ActivityFormTypeId &&
//               (NamespaceId ?? "") == (activityAuthorization.ActivityNamespaceId ?? "") &&
//               (EntityId ?? "") == (activityAuthorization.ActivityEntityId ?? "") &&
//               (FormSubjectId ?? "") == (activityAuthorization.ActivityFormSubjectId ?? "") &&
//               (Controller ?? "") == (activityAuthorization.ActivityControllerId ?? "") &&
//               (Action ?? "") == (activityAuthorization.ActivityControllerAction ?? "") &&
//               (long)SystemFeature == activityAuthorization.SystemFeatureId;
//    }

//    public UserActivityAuthorization GetSystemUserActivity(string assigneeId)
//    {
//        return new UserActivityAuthorization
//        {
//            Id = Id ?? 0,
//            UserId = assigneeId,
//            ActivityTypeId = (long)ActivityType,
//            ActivityFormTypeId = (long)FormType,
//            ActivityNamespaceId = NamespaceId,
//            ActivityEntityId = EntityId,
//            EntityItemId = EntityItemId,
//            ActivityControllerId = Controller,
//            ActivityControllerAction = Action,
//            Disable = IsDiactivated,
//            SystemFeatureId = (long)SystemFeature,
//            ActivityFormSubjectId = FormSubjectId,
//        };
//    }

//    public UserGroupActivityAuthorization GetSystemUserGroupActivity(string assigneeId)
//    {
//        return new UserGroupActivityAuthorization
//        {
//            Id = Id ?? 0,
//            UserGroupId = Convert.ToInt64(assigneeId),//todo Please check
//            ActivityTypeId = (long)ActivityType,
//            ActivityFormTypeId = (long)FormType,
//            ActivityNamespaceId = NamespaceId,
//            ActivityEntityId = EntityId,
//            EntityItemId = EntityItemId,
//            ActivityControllerId = Controller,
//            ActivityControllerAction = Action,
//            Disable = IsDiactivated,
//            SystemFeatureId = (long)SystemFeature,
//            ActivityFormSubjectId = FormSubjectId,
//            //                OnlyAuthority = 
//        };
//    }
//}

//public class ControllerComparer : IEqualityComparer<AssignAccessItemViewModel>
//{
//    public bool Equals(AssignAccessItemViewModel x, AssignAccessItemViewModel y)
//    {
//        return x?.Controller == y?.Controller && x?.Action == y?.Action;
//    }

//    public int GetHashCode(AssignAccessItemViewModel obj)
//    {
//        return new
//        {
//            A = obj.Controller,
//            B = obj.Action
//        }.GetHashCode();
//    }
//}

//public enum AssigneeType
//{
//    User,
//    UserGroup
//}
