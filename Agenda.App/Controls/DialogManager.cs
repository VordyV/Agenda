using System;
using System.Threading.Tasks;
using Agenda.Core;
using Agenda.Forms;
using Avalonia.Controls;
using Ursa.Controls;
using Form = Agenda.Forms.Form;

namespace Agenda.Controls;

public static class DialogManager
{
    public static void ShowOverlay(Func<DialogContext, Form> form, bool canDragMove = false, bool canResize = false, bool fullscreen = false)
    {
        var context = new DialogContext();
        OverlayDialog.ShowCustom(form.Invoke(context), context, hostId: "main", new OverlayDialogOptions() {CanDragMove = canDragMove, CanResize = canResize, FullScreen = fullscreen});
    }
    
    public static async Task ShowOverlayModal(Func<DialogContext, Form> form, bool canDragMove = false, bool canResize = false, bool fullscreen = false)
    {
        var context = new DialogContext();
        await OverlayDialog.ShowCustomAsync<Form>(form.Invoke(context), context, hostId: "main", new OverlayDialogOptions() {CanDragMove = canDragMove, CanResize = canResize, FullScreen = fullscreen});
    }
}