   M S B A M L   `   `   `  ���� ?  ;Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:CoreCore      Core.ChooseParent   W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      8x,http://schemas.microsoft.com/winfx/2006/xaml    >d2http://schemas.microsoft.com/expression/blend/2008    5c-http://schemas.microsoft.com/wpf/2008/toolkit  localclr-namespace:Core  -     � Title$'  !Выберите Документ��$	��396��$	��681��$	��396��$	��680��$��14(� � 	Resources   Core.DateToColorConverter   GridColorConverter��%   &	          .��� .+�)  #Microsoft.Windows.Controls.DataGrid  -   $��gridDocuments��$��	0,0,186,0q�   AutoGenerateColumns .     CanUserAddRows .     CanUserDeleteRows .     
IsReadOnly .   	Documents � DataContext# �     GridLinesVisibility$
 None=�����    Columns 3  -Microsoft.Windows.Controls.DataGridTextColumn  /  )Microsoft.Windows.Controls.DataGridColumn	   Header$	 № Документа��
   Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$
 * 4  .Microsoft.Windows.Controls.DataGridBoundColumn   Binding �� *
DocumentId+  $!	 Дата Документа��$
 *  �� *DocumentDate+ �� StringFormat$ {0:dd.MM.yyyy}��  $	 Объект��$
 *  �� *
UnitName+  $	 
Сумма��$
 *  �� *Total+$
 0.00��  $	 Тип Документа��$
 *  �� *DocTypeName+   RowStyle ��  �� 
TargetType,  &Microsoft.Windows.Controls.DataGridRow"  .���  �� Property � �� �� Value �� *DocumentDate+ �� 	Converter# [  �� -   $��	btnChoose��$��Выбрать��$��Right=�$��85��$��27��$��Bottom=�$��0,0,6,6q��� -   $��btnClose��$��Закрыть��$��27��$��0,0,95,6q�$��Bottom=�$��Right=�$��85���� -   $��	btnSearch��$��
Поиск��$��25��$��Right=�$	��Top=�$��94��$��
0,171,43,0q�)  #Microsoft.Windows.Controls.Calendar  -   $��cldrDate��   SelectionMode$ SingleRange=�   DisplayMode$ Month=�$	��171��$	��Top=�$��Right=�$	��188��