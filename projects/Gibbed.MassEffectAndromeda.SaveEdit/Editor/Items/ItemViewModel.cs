using Caliburn.Micro;
using Gibbed.MassEffectAndromeda.SaveFormats.Items;

namespace Gibbed.MassEffectAndromeda.SaveEdit.Items
{
    internal class ItemViewModel : PropertyChangedBase
    {
        #region Fields
        private readonly uint _Id;
        private readonly ItemData _Item;
        #endregion

        public ItemViewModel(uint id, ItemData item)
        {
            this._Id = id;
            this._Item = item;
        }

        #region Properties
        public GameInfo.ItemDefinition Definition
        {
            get { return this._Item.Definition; }
        }

        public uint Id
        {
            get { return this._Id; }
        }

        public ItemData Data
        {
            get { return this._Item; }
        }

        public bool UsesTier
        {
            get
            {
                return this._Item.Definition.Type == GameInfo.ItemType.MeleeWeapon ||
                       this._Item.Definition.Type == GameInfo.ItemType.RangedWeapon ||
                       this._Item.Definition.Type == GameInfo.ItemType.Gear ||
                       this._Item.Definition.Type == GameInfo.ItemType.Research;
            }
        }

        public virtual string DisplayName
        {
            get { return this.GetDisplayName(); }
        }

        public string DisplayGroup
        {
            get { return GetDisplayGroup(this.Definition.Type); }
        }

        public int DisplayGroupOrder
        {
            get { return GetDisplayGroupOrder(this.Definition.Type); }
        }

        public uint Unknown1
        {
            get { return this._Item.Unknown1; }
            set
            {
                this._Item.Unknown1 = value;
                this.NotifyOfPropertyChange(() => this.Unknown1);
            }
        }

        public string PartitionName
        {
            get { return this._Item.PartitionName; }
        }

        public int Quantity
        {
            get { return this._Item.Quantity; }
            set
            {
                this._Item.Quantity = value;
                this.NotifyOfPropertyChange(() => this.Quantity);
            }
        }

        public bool IsNew
        {
            get { return this._Item.IsNew; }
            set
            {
                this._Item.IsNew = value;
                this.NotifyOfPropertyChange(() => this.IsNew);
            }
        }

        public int Rarity
        {
            get { return this._Item.Rarity; }
            set
            {
                this._Item.Rarity = value;
                this.NotifyOfPropertyChange(() => this.Rarity);
            }
        }
        #endregion

        private static string GetDisplayGroup(GameInfo.ItemType type)
        {
            return type.GetDisplayName() ?? "#" + type;
        }

        private int GetDisplayGroupOrder(GameInfo.ItemType type)
        {
            return type.GetDisplayOrder();
        }

        private string GetDisplayName()
        {
            var name = string.IsNullOrEmpty(this._Item.Definition.Name)
                           ? this._Item.PartitionName
                           : this._Item.Definition.Name;

            if (this.UsesTier == true)
            {
                var tier = this._Item.Definition.Tier;
                if (tier >= 0)
                {
                    switch (tier)
                    {
                        case 0:
                        {
                            name += " I";
                            break;
                        }

                        case 1:
                        {
                            name += " II";
                            break;
                        }

                        case 2:
                        {
                            name += " III";
                            break;
                        }

                        case 3:
                        {
                            name += " IV";
                            break;
                        }

                        case 4:
                        {
                            name += " V";
                            break;
                        }

                        case 5:
                        {
                            name += " VI";
                            break;
                        }

                        case 6:
                        {
                            name += " VII";
                            break;
                        }

                        case 7:
                        {
                            name += " VIII";
                            break;
                        }

                        case 8:
                        {
                            name += " IX";
                            break;
                        }

                        case 9:
                        {
                            name += " X";
                            break;
                        }

                        default:
                        {
                            name += " #" + tier;
                            break;
                        }
                    }
                }
            }
            return name;
        }
    }
}
