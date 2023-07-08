using EasyButtons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[AddComponentMenu("UI/Menus/Menu Handler", order: 0)]
[RequireComponent(typeof(PlayerInput))]
public class MenuHandlerSingleton : MonoBehaviour
{
    public static MenuHandlerSingleton Singleton;
    public string NoMenuActionMap;
    public List<MenuBehaviour> Menus;
    PlayerInput playerInput;
    MenuBehaviour activeMenu;
    MenuBehaviour previousMenu;
    float timePassed = 0f;
    bool recentlyChangedMenus = false;

    void Awake() {
        playerInput = GetComponent<PlayerInput>();
        if (Singleton == null) {
            Singleton = this;
        } else {
            Destroy(this);
            return;
        }
    }

    private void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed > 0.5f)
        {
            recentlyChangedMenus = false;
            timePassed = 0f;
        }
    }

    public void ChangeMenuInstantly(string name) => changeMenu(name);

    /// <summary>Disables all other canvases, and selects the canvas first Button</summary>
    void changeMenu(string name, bool updatePrevious = true) {
        int index = Menus.FindIndex((MenuBehaviour menu) => menu.MenuName == name);
        if (index < 0) {
            Debug.LogError($"The menu '{name}' does not exist");
            return;
        }

        if (Menus.Count == 0f) {
            Debug.LogError("No Canvases given to MenuHandler");
            return;
        }
        if (activeMenu != null && Menus[index] == activeMenu) return;
        if (activeMenu != null && updatePrevious) previousMenu = activeMenu;
        DisableAllMenus();

        MenuBehaviour menu = Menus[index];
        activeMenu = menu;
        recentlyChangedMenus = true;
        menu.gameObject.SetActive(true);
        menu.GetFirstSelectable().Select();
    }

    public void DisableAllMenus() {
        for (int i = 0; i < Menus.Count; i++)
            Menus[i].gameObject.SetActive(false);
    }

    public void EnableActiveMenu()
    {
        if (activeMenu == null) return;
        activeMenu.gameObject.SetActive(true);
    }

    public void DisableActiveMenu()
    {
        if (activeMenu == null) return;
        activeMenu.gameObject.SetActive(false);
    }

    public void ChangeToPreviousMenu()
    {
        if (recentlyChangedMenus) return;
        CloseMenu(activeMenu);
        if (previousMenu != null)
            changeMenu(previousMenu.MenuName, false);
    }

    public void CloseMenu(MenuBehaviour menu)
    {
        if (menu == null)
            return;
        menu.gameObject.SetActive(false);
        if (menu == activeMenu)
            activeMenu = null;

        if (previousMenu == null)
        {
            playerInput.SwitchCurrentActionMap("Player");
            return;
        }
    }

    public void ChangeMenu(string menuName) => ChangeMenu(menuName, true);

    public void ChangeMenu(string menuName, bool updatePrevious = true) { 
        if (activeMenu != null && activeMenu.MenuName == menuName) return;
        changeMenu(menuName, updatePrevious);
    }

    [Button]
    public void SeeCurrentlySelected() {
        Debug.Log("Currently selected object: " + EventSystem.current.currentSelectedGameObject.name, EventSystem.current.currentSelectedGameObject);
    }
}
