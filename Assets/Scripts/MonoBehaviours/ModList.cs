using System.Collections.Generic;
using System.Linq;
using LethalConfig.Mods;
using LethalConfig.MonoBehaviours.Managers;
using UnityEngine;

namespace LethalConfig.MonoBehaviours
{
    internal class ModList : MonoBehaviour
    {
        public GameObject modItemPrefab;
        public GameObject listContainerObject;

        public ConfigList configList;
        public DescriptionBox descriptionBox;

        private List<ModListItem> _items;

        private void Awake()
        {
            _items = new List<ModListItem>();
            BuildModList();
        }

        private void BuildModList()
        {
            _items.Clear();
            foreach (Transform child in listContainerObject.transform) Destroy(child.gameObject);

            var mods = LethalConfigManager.Mods.Values
                .OrderBy(m => m.ModInfo.Guid != PluginInfo.Guid)
                .ThenBy(m => m.IsAutoGenerated)
                .ThenBy(m => m.ModInfo.Name);

            foreach (var mod in mods)
            {
                var modItem = Instantiate(modItemPrefab, listContainerObject.transform);
                modItem.transform.localScale = Vector3.one;
                modItem.transform.localPosition = Vector3.zero;
                modItem.transform.localRotation = Quaternion.identity;
                var listItem = modItem.GetComponent<ModListItem>();
                listItem.Mod = mod;
                listItem.ModSelected += ModSelected;
                listItem.OnHoverEnter += () =>
                {
                    descriptionBox.ShowModInfo(mod);
                    ConfigMenuManager.Instance.menuAudio.PlayHoverSfx();
                };
                listItem.OnHoverExit += () => { descriptionBox.HideModInfo(); };
                _items.Add(modItem.GetComponent<ModListItem>());
            }
        }

        private void ModSelected(Mod mod)
        {
            ConfigMenuManager.Instance.menuAudio.PlayConfirmSfx();
            configList.LoadConfigsForMod(mod);

            _items.First(i => i.Mod == mod).SetSelected(true);

            foreach (var item in _items.Where(i => i.Mod != mod))
                item.SetSelected(false);
        }
    }
}