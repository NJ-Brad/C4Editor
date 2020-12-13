// https://searchcode.com/codesearch/view/28354570/
/******************************************************************************/
/*                                                                            */
/*    _                ___        _..-._                                      */
/*    \`.|\..----...-'`   `-._.-'' _.-..'                                     */
/*    /  ' `         ,       __.-''                                           */
/*    )/` _/     \   `-_,   /                                                 */
/*    `-'" `"\_  ,_.-;_.-\_ ',  NABU Project                                  */
/*        _.-'_./   {_.'   ; /  Nabu.Forms Library                            */
/*       {_.-``-'         {_/   Copyright � TrifleSoft, 2005-2012             */
/*                                                                            */
/******************************************************************************/
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

//using Nabu.Platform;

namespace Nabu.Forms
{
	internal class SplitButton : Button
	{
		#region SplitButton

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ContextMenuStrip _dropDownMenu;

		#endregion Fields

		#region Methods

		#region Event Raisers

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnDropDownMenuChanged(EventArgs e)
		{
			var eventHandler = DropDownMenuChanged;

			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnSplitClick(CancelEventArgs e)
		{
			var eventHandler = SplitClick;

			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		#endregion Event Raisers

		#region Constructors

		public SplitButton()
		{
			FlatStyle = FlatStyle.System;
		}

		#endregion Constructors

		#endregion Methods

		#region Properties

		[Description("Gets or sets the dropdown menu associated with the splitbutton.")]
		[Category("Behavior")]
		[DefaultValue("")]
		public ContextMenuStrip DropDownMenu
		{
			[DebuggerStepThrough]
			get { return _dropDownMenu; }

			[DebuggerStepThrough]
			set
			{
				_dropDownMenu = value;
				//OnContextMenuChanged(EventArgs.Empty);
			}
		}

		#endregion Properties

		#region Events

		[Category("Property Changed")]
		[Description("Occurs when the value of the Nabu.Forms.SplitButton.DropDownMenu property has changed.")]
		public event EventHandler DropDownMenuChanged;

		[Category("Action")]
		[Description(" Occurs when the split part of button is clicked.")]
		public event CancelEventHandler SplitClick;

		#endregion Events

		#endregion SplitButton

		#region Button

		protected override CreateParams CreateParams
		{
			get
			{
				var createParams = base.CreateParams;
				const int BS_SPLITBUTTON = 0x0000000C;
				const int BS_DEFSPLITBUTTON = 0x0000000D;

//				if (Environment.OSVersion.Version >= OSVersion.WindowsVista)
				{
					createParams.Style |= (IsDefault ? BS_DEFSPLITBUTTON : BS_SPLITBUTTON);
				}

				return createParams;
			}
		}

		protected override void WndProc(ref Message message)
		{
			const int BCM_SETDROPDOWNSTATE = 0x1606;

			switch (message.Msg)
			{
				case BCM_SETDROPDOWNSTATE:
					if (message.WParam.ToInt32() == 1)
					{
						var eventArgs = new CancelEventArgs();

						OnSplitClick(eventArgs);

						if ((!eventArgs.Cancel) && (_dropDownMenu != null))
						{
							_dropDownMenu.Show(this, 0, Height);
						}
					}
					break;
			}

			base.WndProc(ref message);
		}

		#endregion Button
	}
}
/******************************************************************************/
/*                                END OF FILE                                 */
/******************************************************************************/