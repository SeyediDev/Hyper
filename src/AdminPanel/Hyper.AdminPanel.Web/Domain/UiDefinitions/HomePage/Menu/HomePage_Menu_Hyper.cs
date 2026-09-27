using Neo.Domain.Entities.Common;
using User = Hyper.Domain.Entities.Common.User;

namespace Hyper.AdminPanel.Domain.UiDefinitions.HomePage;

public partial class HomePageEntityUiDefinitions
{
    public partial class Default
    {
        private bool _state_is_form;

        private void AddHyperMenu()
        {
            AddMultiTabItem("پنل مدیریتی هایپریک", nameof(DomainProvider.Domain), ContextualStyle.Secondary, nameof(DomainProvider.Domain));
            _state_is_form = true;
            AddUsersHomePageMenuPart();
            _state_is_form = false;
            AddUsersHomePageMenuPart();
            EndPart();
        }

        private void AddUsersHomePageMenuPart()
        {
            AddHyperPart("کاربران", "UsersInfo");
            {
                AddHyperPartItem<User>("کاربران");
                EndPart();
            }
            AddHyperPart("اطلاعات پایه", "Public Info");
            {
                AddHyperPartItem<CultureTerm>("واژه ها");
                AddHyperPartItem<Country>("کشورها");
                AddHyperPartItem<Province>("استان‌ها");
                AddHyperPartItem<City>("شهرها");
                EndPart();
            }
            AddHyperPart("مستندات", "DocumentsInfo");
            {
                AddHyperPartItem<Hyper.Domain.Entities.Common.DocumentType>("نوع مستندات");
                AddHyperPartItem<Document>("مستندات");
                EndPart();
            }
        }

        void AddHyperPartItem<T>(string name, string? subject = null, string? pageSubType = null, string? entityItemId = null, string partName = nameof(Domains.Hyper))
        {
            if (_state_is_form)
            {
                AddPartForm<T>(name, subject, pageSubType, partName, entityItemId);
            }
            else
            {
                AddPartReport<T>(name, subject, partName, entityItemId);
            }
        }

        private void AddHyperPart(string name, string enName)
        {
            AddPart((_state_is_form ? "" : "گزارشات ") + name,
                "IUM " + enName + (_state_is_form ? "-Form" : "-Report"), _state_is_form ? "#FFF" : "#CCE2FF");
        }
    }
}
