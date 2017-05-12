using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.Globalization;
using Sitecore.SecurityModel;
using System;

namespace Sitecore.Support.Tasks
{
    public class ItemEventHandler : Sitecore.Tasks.ItemEventHandler
    {
        private void Fill(Item item)
        {
            foreach (Language language in item.Languages)
            {
                Item item2 = item.Database.GetItem(item.ID, language);
                if (item2.Versions.Count > 0)
                {
                    Item latestVersion = item2.Versions.GetLatestVersion();
                    latestVersion.Editing.BeginEdit();
                    using (new EventDisabler())
                    {
                        latestVersion.Fields[FieldIDs.Created].SetValue(DateUtil.IsoNowWithTicks, false);
                        latestVersion.Fields[FieldIDs.Updated].SetValue(DateUtil.IsoNowWithTicks, false);
                        latestVersion.Fields[FieldIDs.CreatedBy].SetValue(Context.User.Name, false);
                    }
                    latestVersion.Editing.EndEdit();
                }
            }
            foreach (Item item4 in item.GetChildren())
            {
                this.Fill(item4);
            }
        }

        protected void UpdateStatisticsInfo(object sender, EventArgs args)
        {
            Item item = Event.ExtractParameter(args, 1) as Item;
            Error.AssertNotNull(item, "No item in parameters");
            using (new SecurityDisabler())
            {
                this.Fill(item);
            }
        }
    }
}
