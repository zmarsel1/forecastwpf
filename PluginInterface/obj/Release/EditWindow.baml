   M S B A M L   `   `   `  ���� J  FPluginInterface, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:PluginInterfacePluginInterface       PluginInterface.EditWindow   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     8x,http://schemas.microsoft.com/winfx/2006/xaml    >d2http://schemas.microsoft.com/expression/blend/2008    5c-http://schemas.microsoft.com/wpf/2008/toolkit  )localclr-namespace:PluginInterface  -    	  Title  � Title#  [  $	��350��$	��600��$	��405��$	��600��$��14(� � SizeToContent$ Height=� � 	Resources ��%    .��� .+��� -   $��docInfo��  Info#��[ $��67��$��	7,0,151,0q�$	��Top=���   Sum#���  $��0,37,0,0q�$��Right=�$	��142��$��30��$	��Top=�$��Right=� �� ContentStringFormat$
 0.00��)  #Microsoft.Windows.Controls.DataGrid  -   $��gridData��  Data � DataContext# �  $��	0,73,0,34q�   AutoGenerateColumns .     CanUserAddRows .     CanUserDeleteRows .  $��Tahoma)�   SelectionMode$ Single=�	   GridLinesVisibility$	 Vertical=����� *Body+
   Columns
 3  -Microsoft.Windows.Controls.DataGridTextColumn  /  )Microsoft.Windows.Controls.DataGridColumn   Header$ 
Время��   
IsReadOnly .    SortMemberPath$ RowKey��   Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$ * 4  .Microsoft.Windows.Controls.DataGridBoundColumn   Binding �� *RowKey+  RowConverter �� 	Converter# [ 7  1Microsoft.Windows.Controls.DataGridComboBoxColumn  $ Продукция��$ *    DisplayMemberPath$ 
EntityName��   SelectedValuePath$ 	EntityKey��   ItemsSource ��  �� Source# [  �� Path$ Entities�   SelectedValueBinding �� *	EntityKey+  $ Значение��$ *  �� *
EntityData+ �� StringFormat$
 0.00��   	CellStyle ��  �� 
TargetType-  'Microsoft.Windows.Controls.DataGridCell"  .���  �� Property �@��TextAlignment �� Value$ Right=� �� Triggers P�  P� Property �@ 
IsSelected P� Value . . ���  � ��
 ��ح���  � ��
 �   ���  � ��
 ��ح�   	CellStyle �� "  .�-    P�  �@ 
IsSelected . . ���  � ��
 ��ح���  � ��
 �   ���  � ��
 ��ح��� -   $��
chkCascade��$-��'Каскадное обновление��$
��Left=�$��7,0,0,12q�$��16��$��Bottom=�$	��169���� -   $��btnSave��$��Сохранить��$��0,0,3,5q�$��Right=�$��90��$��27��$��Bottom=��� -   $��btnClose��$��Закрыть��$��0,0,99,5q�$��Right=�$��75��$��27��$��Bottom=�