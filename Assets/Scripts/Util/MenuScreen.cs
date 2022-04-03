using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[SelectionBase]
[RequireComponent(typeof(CanvasGroup))]
public class MenuScreen : MonoBehaviour {

    enum ShowAction {
        NONE, SHOW, HIDE
    }

    [SerializeField] Selectable selectOnShow;
    [SerializeField] private MenuScreenGroup _menuScreenGroup;
    [SerializeField] bool recenterOnAwake = true;
    [SerializeField] bool showOnTop = true;
    [SerializeField] ShowAction showOnStart = ShowAction.NONE;

    [ReadOnly] public bool isShown = false;

    [Header("Events")]
    // for stuff like animation?
    public UnityEvent OnShownEvent;
    public UnityEvent OnHiddenEvent;

    CanvasGroup canvasGroup;

    public MenuScreenGroup menuScreenGroup {
        get => _menuScreenGroup;
        set {
            // in case changed before enable
            if (menuScreenGroup != null && menuScreenGroup.allMenuScreens.Contains(this)) {
                menuScreenGroup.UnRegisterMenuScreen(this);
            }
            _menuScreenGroup = value;
            if (menuScreenGroup != null) {
                menuScreenGroup.RegisterMenuScreen(this);
            }
        }
    }

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
        if (recenterOnAwake) {
            RecenterPosition();
        }
    }
    private void Start() {
        if (showOnStart == ShowAction.SHOW) {
            // changed so will invoke event
            isShown = false;
            Show();
        } else if (showOnStart == ShowAction.HIDE) {
            isShown = true;
            Hide();
        }
    }
    private void OnEnable() {
        if (menuScreenGroup != null && !menuScreenGroup.allMenuScreens.Contains(this)) {
            menuScreenGroup.RegisterMenuScreen(this);
        }
    }
    private void OnDisable() {
        if (menuScreenGroup != null) {
            menuScreenGroup.UnRegisterMenuScreen(this);
        }
    }

    [ContextMenu("Recenter pos")]
    public void RecenterPosition() {
        var rt = transform as RectTransform;
        rt.localPosition = Vector3.zero;
    }
    [ContextMenu("Show")]
    void ShowEditor() {
        canvasGroup = GetComponent<CanvasGroup>();
        SetShownDontNotify(true);
    }
    [ContextMenu("Hide")]
    void HideEditor() {
        canvasGroup = GetComponent<CanvasGroup>();
        SetShownDontNotify(false);
    }
    public void Show() {
        SetShown(true);
    }
    public void Hide() {
        SetShown(false);
    }
    public void SetShown(bool shown) {
        SetShownDontNotify(shown);
        if (shown) {
            if (menuScreenGroup != null) {
                menuScreenGroup.NotifyMenuScreenOn(this);
            }
        }
    }
    public void SetShownDontNotify(bool shown) {
        bool wasShown = shown;
        isShown = shown;
        canvasGroup.alpha = shown ? 1f : 0f;
        canvasGroup.blocksRaycasts = shown;
        canvasGroup.interactable = shown;
        if (isShown) {
            selectOnShow?.Select();
            if (showOnTop) {
                // make sure we are on top of others
                transform.SetAsLastSibling();
            }
            // only invoke events if state changed
            if (!wasShown) {
                OnShownEvent?.Invoke();
            }
        } else {
            if (wasShown) {
                OnHiddenEvent?.Invoke();
            }
        }
    }
}