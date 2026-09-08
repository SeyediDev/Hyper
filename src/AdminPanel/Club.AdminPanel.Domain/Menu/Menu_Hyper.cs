using Hyper.AdminPanel.Domain.UiDefinitions.HomePage;
using Hyper.Domain.Entities.Channels;
using Hyper.Domain.Entities.CustomerSegments.Data;
using Hyper.Domain.Entities.Events.Data;
using Hyper.Domain.Entities.Lotteries;
using Hyper.Domain.Entities.Metrics.Data;
using Hyper.Domain.Entities.Promotions.Plans.Data;
using Hyper.Domain.Entities.Promotions.Surveys.Data;

namespace Hyper.AdminPanel.Domain.Domain;

/// <summary>
/// Hyper platform menu definitions with modern, colorful icons
/// Icons are defined in /Content/custom-icons/custom-sprite.svg
/// </summary>
public partial class HyperMenuDefinitions : MenuDefinition
{
    private void AddMenu_Hyper()
    {
        AddMenu("بینش و داشبوردها", "home-dashboard", "Insights & Dashboards", "");
        {
            StartSubMenus();
            AddMenu<HomePageEntity, HomePageEntityUiDefinitions.HomePageDashboard>("داشبورد کلان", "home-dashboard");
            AddMenu<HomePageEntity, HomePageEntityUiDefinitions.CustomerIntelligenceDashboard>("بینش مشتری", "users-group");
            AddMenu<HomePageEntity, HomePageEntityUiDefinitions.MarketingPerformanceDashboard>("تحلیل بازاریابی", "gift-present");
            AddMenu<HomePageEntity, HomePageEntityUiDefinitions.HomePageDashboard.CommunityMarketDashboard>("جامعه/بازار", "users-group");
            AddMenu<HomePageEntity, HomePageEntityUiDefinitions.OperationsCommandDashboard>("کنترل عملیات", "list-checklist");
            AddMenuDivider();
            AddReport<CustomerTransaction>("گزارش تراکنش‌های مشتریان", "wallet-money");
            AddReport<EventLog>("گزارش فعالیت‌های سیستم", "list-checklist");
            AddReport<Promotion>("گزارش کمپین‌ها", "gift-present");
            AddReport<CustomerAnalytics>("گزارش تحلیل مشتریان", "chart-bar");
            AddReport<MarketAnalysis>("گزارش تحلیل بازار", "chart-bar");
            EndSubMenus();
        }
		AddMenu("اکوسیستم", "calendar-event", "Tenants", "");
		{
			StartSubMenus();
			AddMenu<Tenant>("اکوسیستم", "building-organization");
			AddMenu<EventChannel>("کانال‌های اکوسیستم", "rss-signal");
            AddMenu<TenantAttribute>("ویژگی‌های اکوسیستم", "user-settings");
            AddMenu<MetricDefinition>("تعریف شاخص‌ها", "chart-bar");
			AddMenuDivider();
			AddMenu<EventType>("رویدادها", "calendar-event");
			AddMenuDivider();
			AddMenu("گزارشات", "chart-bar", "Engagement Reports", "");
			{
				StartSubMenus();
				AddReport<EventLog>("گزارش رویدادها", "list-checklist");
				AddReport<EventChannel>("گزارش کانال‌ها", "rss-signal");
                AddReport<TenantAttribute>("ویژگی‌های اکوسیستم", "user-settings");
                AddReport<MetricDefinition>("گزارش تعریف شاخص‌ها", "chart-bar");
				AddReport<MetricValue>("گزارش مقادیر شاخص‌ها", "chart-bar");
                AddMenu("انجمن مشتریان", "users-group", "Community", "");
				{
					StartSubMenus();
					AddMenu<ForumTopic>("موضوعات انجمن", "list-checklist");
					AddMenu<ForumPost>("پست‌های انجمن", "list-checklist");
					AddMenu<ForumTopicLike>("لایک‌های موضوع", "star-badge");
					AddMenu<ForumPostLike>("لایک‌های پست", "star-badge");
					EndSubMenus();
				}
				AddMenu("بازخورد مشتریان", "question-circle", "Feedback", "");
				{
					StartSubMenus();
					AddMenu<CustomerFeedback>("بازخوردها", "question-circle");
					AddMenu<FeedbackComment>("نظرات بازخورد", "question-circle");
					AddMenu<FeedbackAttachment>("پیوست‌های بازخورد", "file-document");
					AddMenu<FeedbackLike>("لایک‌های بازخورد", "star-badge");
					EndSubMenus();
				}
				EndSubMenus();
			}
			EndSubMenus();
		}
		AddMenu("جامعه/بازار", "users-group", "Customer Segmentation", "");
        {
			StartSubMenus();
			AddMenu<CustomerSegment>("جامعه/بازار مشتریان", "users-group");
            AddMenu<CustomerSegmentMembership>("اعضاء", "user-badge");
			AddMenuDivider();
			AddMenu<CustomerAnalytics>("تحلیل مشتریان (RFM)", "chart-bar");
			AddMenu<MarketAnalysis>("تحلیل بازار", "chart-bar");
			AddMenu("گزارش‌های جامعه/بازار", "chart-bar", "Customer Reports", "");
			{
				StartSubMenus();
				AddReport<Tenant>("گزارش اکوسیستم", "building-organization");
				AddReport<CustomerSegment>("گزارش جامعه/بازار", "users-group");
                AddReport<CustomerSegmentMembership>("گزارش اعضاء", "user-badge");
                AddMenuDivider();
				AddReport<CustomerAnalytics>("تحلیل مشتریان (RFM)", "chart-bar");
				AddReport<MarketAnalysis>("تحلیل بازار", "chart-bar");
				EndSubMenus();
			}
			EndSubMenus();
		}
        AddMenu("اطلاعات مشتریان", "users-info", "Customer Information", "");
        {
            StartSubMenus();
            AddMenu<CustomerTenant>("مشتریان اکوسیستم", "users-network");
            AddMenu<Customer>("مشتریان", "user-circle");
            AddMenu<TenantAttributeValue>("مقادیر پارامترهای مشتریان", "sliders-h");
            AddMenu<CustomerTransaction>("تراکنش‌های امتیازی", "wallet-money");
            AddMenu<CustomerPointLevel>("سطوح وفاداری", "trophy-star");
			AddMenuDivider("Engagments", "تعاملات");
            AddMenu<RewardAsset>("خریدهای پاداش", "cube-3d");
            AddMenuDivider("Metrics", "شاخص ها");
            AddMenu<MetricValue>("مقادیر شاخص‌ها", "chart-bar");
            AddMenu<CustomerProductMetrics>("معیارهای مشتری-محصول", "chart-bar");
            AddMenu("معرفی", "users-group", "Referal Information", "");
            {
                StartSubMenus();
                AddMenu<CustomerReferrer>("معرفی‌های مشتری", "users-group");
                AddMenu<ReferrerCode>("کدهای معرف", "users-group");
                AddReport<CustomerReferrer>("گزارشات معرفی‌های مشتری", "users-group");
                AddReport<ReferrerCode>("گزارشات کدهای معرف", "users-group");
                EndSubMenus();
            }
            AddMenu("گزارش‌های اطلاعات مشتری", "chart-bar", "Customer Information Reports", "");
            {
                StartSubMenus();
                AddReport<Customer>("گزارش مشتریان", "user-circle");
                AddReport<CustomerTenant>("گزارش تحلیل مشتریان اکوسیستم", "users-network");
                AddReport<CustomerTransaction>("گزارش تراکنش‌ها", "wallet-money");
                AddReport<CustomerProductMetrics>("گزارش معیارهای مشتری-محصول", "chart-bar");
                AddReport<CustomerPointLevel>("گزارش سطوح وفاداری", "trophy-star");
                AddReport<RewardAsset>("گزارش دارایی‌ها", "cube-3d");
                EndSubMenus();
            }
            EndSubMenus();
        }
        AddMenu("پاداش‌ها", "box-package", "Rewards", "");
        {
            StartSubMenus();
            AddMenu<Reward>("پاداش‌ها", "box-package");
            AddMenu<RewardCategory>("طبقه بندی پاداش‌ها", "grid-layout");
            AddMenu<RewardCost>("قیمت‌گذاری پاداش‌ها", "coins-money");
			AddMenuDivider();
            AddMenu("گزارش‌های پاداش", "chart-bar", "Catalog Reports", "");
            {
                StartSubMenus();
                AddReport<Reward>("گزارش پاداش‌ها", "box-package");
                AddReport<RewardCategory>("طبقه بندی پاداش‌ها", "grid-layout");
                AddReport<RewardCost>("گزارش هزینه‌ها", "coins-money");
                AddReport<Plan>("گزارش طرح‌های اشتراک", "piggy-bank");
                EndSubMenus();
            }
            EndSubMenus();
        }
        AddMenu("محصولات", "grid-layout", "Products", "");
        {
            StartSubMenus();
            AddMenu<Product>("محصولات", "grid-layout");
            AddMenu<ProductCategory>("دسته‌بندی محصول", "grid-layout");
            AddMenu<ProductFitAnalysis>("تناسب محصول", "grid-layout");
            AddMenuDivider();
            AddMenu("گزارش‌های محصول", "chart-bar", "Product Reports", "");
            {
                StartSubMenus();
                AddReport<Product>("گزارش محصولات", "grid-layout");
                AddReport<ProductCategory>("گزارش دسته‌بندی محصولات", "grid-layout");
                AddReport<ProductFitAnalysis>("تناسب محصول", "grid-layout");
                EndSubMenus();
            }
            EndSubMenus();
        }
        AddMenu("امتیازات", "star-badge", "Points", "");
        {
            StartSubMenus();
            AddMenu<Point>("انواع امتیازات", "star-badge");
            AddMenu<PointLevel>("سطوح امتیاز", "medal-reward");
            AddMenu<PointConversionRate>("نرخ تبدیل امتیاز", "coins-money");
            AddMenuDivider();
            AddMenu("گزارش‌های سیستم امتیازدهی", "chart-bar", "Scoring Reports", "");
            {
                StartSubMenus();
                AddReport<Point>("گزارش امتیازات", "star-badge");
                AddReport<PointLevel>("گزارش سطوح‌امتیازی", "medal-reward");
                EndSubMenus();
            }
            EndSubMenus();
        }
        AddMenu("پویش‌ها و کمپین‌ها", "gift-present", "Promotion", "");
        {
            StartSubMenus();
            AddMenu<Promotion>("پویش و کمپین", "gift-present");
            AddMenu<PromotionTrigger>("محرک", "gift-present");
            AddMenu<PromotionAction>("عملیات", "gift-present");
            AddMenu<PromotionCustomerSegment>("جامعه/بازار", "gift-present");
            AddMenu<PromotionBudget>("بودجه", "piggy-bank");
            AddMenu<PromotionCostAllocation>("تسهیم زمانی هزینه", "gift-present");
            AddMenu<Plan>("طرح اشتراک", "piggy-bank");
			AddMenu<Lottery>("قرعه‌کشی و چرخونه", "ticket-lottery");
            AddMenu<Survey>("مسابقه و نظرسنجی", "question-circle");
            AddMenu<PromotionMessage>("پیام", "gift-present");
            AddMenu("گزارش‌های پویش‌ها و کمپین‌ها", "chart-bar", "Engagement Reports", "");
            {
                StartSubMenus();
                AddReport<Promotion>("گزارش پویش‌ها و کمپین‌ها", "gift-present");
                AddReport<PromotionCustomerSegment>("جامعه/بازار", "gift-present");
                AddReport<Lottery>("گزارش قرعه‌کشی", "ticket-lottery");
                AddReport<LotteryReward>("گزارش جوایز قرعه‌کشی", "box-package");
                AddReport<PromotionMessage>("گزارش پیام‌های کمپین", "gift-present");
				AddReport<Survey>("گزارش مسابقه و نظرسنجی", "question-circle");
                AddReport<PromotionBudget>("گزارش بودجه‌ها", "piggy-bank");
                EndSubMenus();
            }
            EndSubMenus();
        }
        AddMenu("اطلاعات پویش‌ها", "gift-present", "Promotion", "");
        {
            StartSubMenus();
            AddMenu<CustomerPlan>("طرح‌های فعال مشتریان", "wallet-money");
            AddMenu<LotteryParticipant>("شرکت‌کنندگان قرعه‌کشی/چرخونه", "users-group");
            AddMenu<SurveyParticipation>("شرکت‌کنندگان نظرسنجی", "users-group");
            AddMenu<PromotionRecipient>("گیرندگان کمپین", "users-group");
            AddMenu("گزارش‌های اطلاعات پویش‌ها", "chart-bar", "Engagement Reports", "");
            {
                StartSubMenus();
                AddReport<CustomerPlan>("گزارش طرح‌های مشتری", "piggy-bank");
                AddReport<LotteryParticipant>("گزارش شرکت‌کنندگان قرعه‌کشی/چرخونه", "users-group");
                AddReport<SurveyParticipation>("گزارش شرکت‌کنندگان در مسابقه و نظرسنجی", "users-group");
                AddReport<PromotionRecipient>("گزارش گیرندگان کمپین", "users-group");
                EndSubMenus();
            }
            EndSubMenus();
        }

        // System Settings
        AddMenu("تنظیمات سامانه", "cog-wheel", "System Settings", "");
        {
            StartSubMenus();
            AddMenu<User>("کاربران سامانه", "user-admin");
            AddMenu<Faq>("سوالات متداول", "question-circle");
            AddMenu<Help>("راهنمای سیستم", "info-circle");
            AddMenu<Document>("مستندات", "file-document");
            AddMenuDivider();
            AddMenu("گزارش‌های تنظیمات سامانه", "chart-bar", "System Reports", "");
            {
                StartSubMenus();
                AddReport<User>("گزارش کاربران", "user-admin");
                AddReport<Faq>("گزارش سوالات متداول", "question-circle");
                AddReport<Help>("گزارش راهنمای سیستم", "info-circle");
                AddReport<Document>("گزارش مستندات", "file-document");
                EndSubMenus();
            }
            EndSubMenus();
        }
    }
}
