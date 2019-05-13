using System.Globalization;
using System.IO;
using DevExpress.Internal;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.RichEdit;
using DevExpress.Xpf.SpellChecker;
using DevExpress.XtraRichEdit.SpellChecker;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraSpellChecker.Native;

namespace DevExpress.DevAV {
    public class SpellCheckerRichEditBehavior : Behavior<RichEditControl>{
        SpellChecker spellChecker;
        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.SpellChecker = spellChecker;
        }
        public SpellCheckerRichEditBehavior() {
            spellChecker = CreateSpellChecker();
        }
        SpellChecker CreateSpellChecker() {
            SpellChecker spellChecker = new SpellChecker();
            spellChecker.Culture = new CultureInfo("en-US");
            spellChecker.SpellCheckMode = SpellCheckMode.AsYouType;
            SpellCheckTextControllersManager.Default.RegisterClass(typeof(RichEditControl), typeof(RichEditSpellCheckController));
            return spellChecker;
        }
    }
}
