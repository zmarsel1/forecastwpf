   M S B A M L   `   `   `  ����?  ;Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:CoreCore  5   	       Core.ChooseParent   5   	   W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   8x,http://schemas.microsoft.com/winfx/2006/xaml    5   	   >d2http://schemas.microsoft.com/expression/blend/2008    5   	   5c-http://schemas.microsoft.com/wpf/2008/toolkit  5   	   localclr-namespace:Core  5   	   -   5          � Title$'  !Выберите Документ��5   	   $	��396��6#   $	��681��60   $	��396��5   	   $	��680��6   $��14(�6(    � 	Resources 5	        Core.DateToColorConverter   GridColorConverter��%   &	          5
   
   6%   5      .��5      � .+�5   
   )  #Microsoft.Windows.Controls.DataGrid  -   5        $��gridDocuments��5      $��	0,0,186,0q�6*      AutoGenerateColumns .     CanUserAddRows .     CanUserDeleteRows .     
IsReadOnly .   	Documents � DataContext# �  5         GridLinesVisibility$
 None=�5      ��5      ��    Columns 5      3  -Microsoft.Windows.Controls.DataGridTextColumn  5      /  )Microsoft.Windows.Controls.DataGridColumn	   Header$	 № Документа��6'   
   Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$
 * 6<   4  .Microsoft.Windows.Controls.DataGridBoundColumn   Binding 6F   �� *
DocumentId+  5      $!	 Дата Документа��6'   $
 * 6?    6I   �� *DocumentDate+ �� StringFormat$ {0:dd.MM.yyyy}��  5      $	 Объект��6'   $
 * 67    6A   �� *
UnitName+  5      $	 
Сумма��6'   $
 * 66    6@   �� *Total+$
 0.00��  5      $	 Тип Документа��6'   $
 * 6>    6H   �� *DocTypeName+   RowStyle 5      �� 5       �� 
TargetType,  &Microsoft.Windows.Controls.DataGridRow"  6   .�5      ��  �� Property � �� �� Value 63   �� *DocumentDate+ �� 	Converter# [  5       5"      �� 5#   
   -   5        $��	btnChoose��5#   ?   $��Выбрать��6   $��Right=�6#   $��85��6P   $��27��6[   $��Bottom=�6g   $��0,0,6,6q�6�   �� 5$   
   -   5        $��btnClose��5$   A   $��Закрыть��6   $��27��6#   $��0,0,95,6q�6/   $��Bottom=�6Q   $��Right=�6�   $��85��6�   �� 5%   
   -   5        $��	btnSearch��5%   I   $��
Поиск��6   $��25��6!   $��Right=�6-   $	��Top=�6Z   $��94��6r   $��
0,171,43,0q�6�   )  #Microsoft.Windows.Controls.Calendar  5&   
   -   5        $��cldrDate��5&         SelectionMode$ SingleRange=�6%      DisplayMode$ Month=�6A   $	��171��6U   $	��Top=�6b   $��Right=�6z   $	��188��6�   6�   5'      5(      