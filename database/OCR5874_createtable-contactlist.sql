- - U S E   [ o w c _ o w e n s ]  
 - - G O  
 / * * * * * *   O b j e c t :     T a b l e   [ d b o ] . [ O C _ C o n t a c t L i s t ]         S c r i p t   D a t e :   0 8 / 2 0 / 2 0 0 8   1 7 : 0 6 : 2 3   * * * * * * /  
 S E T   A N S I _ N U L L S   O N  
 G O  
 S E T   Q U O T E D _ I D E N T I F I E R   O N  
 G O  
 S E T   A N S I _ P A D D I N G   O N  
 G O  
 I F   N O T   E X I S T S   ( S E L E C T   *   F R O M   s y s . o b j e c t s   W H E R E   o b j e c t _ i d   =   O B J E C T _ I D ( N ' [ d b o ] . [ O C _ C o n t a c t L i s t ] ' )   A N D   t y p e   i n   ( N ' U ' ) )  
 B E G I N  
 C R E A T E   T A B L E   [ d b o ] . [ O C _ C o n t a c t L i s t ] (  
 	 [ p k _ i d ]   [ i n t ]   I D E N T I T Y ( 1 , 1 )   N O T   N U L L ,  
 	 [ d a t e _ c r e a t e d ]   [ d a t e t i m e ]   N O T   N U L L   C O N S T R A I N T   [ D F _ o c _ c o n t a c t l i s t _ d a t e _ c r e a t e d ]     D E F A U L T   ( g e t d a t e ( ) ) ,  
 	 [ b u s i n e s s _ a r e a ]   [ n v a r c h a r ] ( 5 0 )   N U L L ,  
 	 [ s o u r c e _ f o r m _ n a m e ]   [ n v a r c h a r ] ( 2 5 5 )   N U L L ,  
 	 [ s o u r c e _ f o r m _ p a t h ]   [ n v a r c h a r ] ( 7 6 8 )   N U L L ,  
 	 [ e x t e r n a l _ k e y ]   [ n v a r c h a r ] ( 5 0 )   N U L L ,  
 	 [ e x t e r n a l _ d a t e ]   [ d a t e t i m e ]   N U L L   C O N S T R A I N T   [ D F _ o c _ c o n t a c t l i s t _ e x t e r n a l _ d a t e ]     D E F A U L T   ( g e t d a t e ( ) ) ,  
 	 [ c o n t a c t _ t y p e ]   [ n v a r c h a r ] ( 2 5 5 )   N U L L ,  
 	 [ c o n t a c t _ f u l l n a m e ]   [ n v a r c h a r ] ( 2 5 5 )   N U L L ,  
 	 [ c o n t a c t _ e m a i l ]   [ n v a r c h a r ] ( 2 5 5 )   N U L L ,  
 	 [ c o n t a c t _ p h o n e ]   [ v a r c h a r ] ( 4 0 )   N U L L ,  
 	 [ c o m p a n y _ n a m e ]   [ n v a r c h a r ] ( 8 0 )   N U L L ,  
 	 [ l a n g u a g e ]   [ n v a r c h a r ] ( 1 5 )   N U L L   C O N S T R A I N T   [ D F _ o c _ c o n t a c t l i s t _ l a n g u a g e ]     D E F A U L T   ( ' e n ' ) ,  
 	 [ x m l _ f o r m _ d a t a ]   [ n t e x t ]   N U L L  
 )   O N   [ P R I M A R Y ]   T E X T I M A G E _ O N   [ P R I M A R Y ]  
 E N D  
 G O  
 S E T   A N S I _ P A D D I N G   O F F 