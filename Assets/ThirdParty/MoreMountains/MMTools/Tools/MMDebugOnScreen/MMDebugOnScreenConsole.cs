using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Tools
{
    public class MMDebugOnScreenConsole : MonoBehaviour
    {
        protected const string space = " ";

        [Header("Bindings")] public RectTransform Container;

        public Image BackgroundImage;
        public Text ConsoleText;

        [Header("Label")] public Color LabelColor = Color.white;

        [Header("Value")] public string ValueColor = "#FFC400";

        public float ValueSizeRatio = 1.35f;

        protected Vector2 _closedSize = new(60, 80);
        protected int _largestMessageLength;
        protected int _last_append_at_frame = -1;
        protected bool _messageStackHasBeenDisplayed;
        protected bool _newMessageThisFrame;

        protected int _numberOfMessages;
        protected Vector2 _openBackgroundWidth;

        protected RectTransform _rectTransform;
        protected StringBuilder _stringBuilder;
        protected string _valueTagEnd;

        protected string _valueTagStart;

        protected virtual void Awake()
        {
            Initialization();
        }

        /// <summary>
        ///     Draws a box containing the current stack of messages on top of the screen.
        /// </summary>
        protected virtual void LateUpdate()
        {
            // we set our flag to true, which will trigger the reset of the stack next time it's accessed
            _messageStackHasBeenDisplayed = true;
            if (!_newMessageThisFrame && ConsoleText.isActiveAndEnabled) gameObject.SetActive(false);
            _newMessageThisFrame = false;
        }

        public virtual void Toggle()
        {
            if (ConsoleText.enabled)
            {
                _openBackgroundWidth = BackgroundImage.rectTransform.sizeDelta;
                BackgroundImage.rectTransform.sizeDelta = _closedSize;
            }
            else
            {
                BackgroundImage.rectTransform.sizeDelta = _openBackgroundWidth;
            }

            ConsoleText.enabled = !ConsoleText.isActiveAndEnabled;
        }

        protected virtual void Initialization()
        {
            ConsoleText.color = LabelColor;
            _stringBuilder = new StringBuilder();
            _rectTransform = gameObject.GetComponent<RectTransform>();

            _valueTagEnd = "</size></color>";
        }

        /// <summary>
        ///     Sets the size of the font, and automatically deduces the character's height and width.
        /// </summary>
        /// <param name="fontSize">Font size.</param>
        protected virtual void SetFontSize(int fontSize)
        {
            if (fontSize == ConsoleText.fontSize) return;
            ConsoleText.fontSize = fontSize;
            _valueTagStart = "<color=" + ValueColor + "><size=" + ConsoleText.fontSize * ValueSizeRatio + ">";
        }

        /// <summary>
        ///     Sets the screen offset, from the top left corner
        /// </summary>
        /// <param name="top"></param>
        /// <param name="left"></param>
        public virtual void SetScreenOffset(int top = 10, int left = 10)
        {
            Container.MMSetTop(top);
            Container.MMSetLeft(left);
        }

        /// <summary>
        ///     Replaces the content of the current message stack with the specified string
        /// </summary>
        /// <param name="newMessage">New message.</param>
        public virtual void SetMessage(string newMessage)
        {
            AddMessage(newMessage, "", 30);
        }

        /// <summary>
        ///     Adds the specified message to the message stack.
        /// </summary>
        /// <param name="label">New message.</param>
        public virtual void AddMessage(string label, object value, int fontSize)
        {
            if (!gameObject.activeInHierarchy) gameObject.SetActive(true);

            var frame = Time.frameCount;

            if (!ConsoleText.isActiveAndEnabled) return;
            _newMessageThisFrame = true;
            SetFontSize(fontSize);

            // if the message stack has been displayed, we empty it and reset our counters
            if (_last_append_at_frame != frame)
            {
                _stringBuilder.Clear();
                _messageStackHasBeenDisplayed = false;
                _numberOfMessages = 0;
                _largestMessageLength = 0;
            }

            _last_append_at_frame = Time.frameCount;

            // we add the specified message to the stack            
            if (_stringBuilder.Length != 0) _stringBuilder.Append(Environment.NewLine);
            _stringBuilder.Append(label.ToUpper());
            _stringBuilder.Append(space);
            _stringBuilder.Append(_valueTagStart);
            _stringBuilder.Append(value);
            _stringBuilder.Append(_valueTagEnd);
            // if this new message is longer than our previous longer message, we store it (this will expand the box's width
            if (label.Length > _largestMessageLength) _largestMessageLength = label.Length;
            // we increment our counter
            _numberOfMessages++;

            ConsoleText.text = _stringBuilder.ToString();
        }
    }
}