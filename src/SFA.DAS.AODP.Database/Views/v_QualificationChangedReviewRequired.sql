��C R E A T E   v i e w   [ r e g u l a t e d ] . [ v _ Q u a l i f i c a t i o n C h a n g e d R e v i e w R e q u i r e d ]   a s 
 
 
 
 / * # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # 
 
 	 - N a m e : 	 	 	 	 Q u a l i f i c a t i o n   N e w   R e v i e w   R e q u i r e d 
 
 	 - D e s c r i p t i o n : 	 	 S h o w s   n e w l y   c r e a t e d   q u a l i f i c a t i o n s   w h i c h   h a v e   n o t   b e e n   s e e n   b y   t h e   s y s t e m 
 
 	 	 	 	 	 	 p r e v i o u s l y ,   t h e s e   a r e   f l a g g e d   i n   t h e   L i f e c y c l e   S t a g e   a s   ' N e w ' 
 
 	 - D a t e   o f   C r e a t i o n : 	 3 1 / 0 1 / 2 0 2 5 
 
 	 - C r e a t e d   B y : 	 	 A d a m   L e a v e r   ( F u j i t s u ) 
 
 # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # 
 
 	 V e r s i o n   N o . 	 	 	 U p d a t e d   B y 	 	 U p d a t e d   D a t e 	 	 D e s c r i p t i o n   o f   C h a n g e 
 
 # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # 
 
 	 1 	 	 	 	 	 A d a m   L e a v e r 	 	 3 1 / 1 2 / 2 0 2 5 	 	 	 O r i g i n a l 
 
 # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # * / 
 
 
 
 s e l e c t   
 
 	 	 a g g . Q u a l i f i c a t i o n R e f e r e n c e 
 
 	 	 , a g g . A w a r d i n g O r g a n i s a t i o n 
 
 	 	 , a g g . Q u a l i f i c a t i o n T i t l e 
 
 	 	 , a g g . Q u a l i f i c a t i o n T y p e 
 
 	 	 , a g g . L e v e l 
 
 	 	 , a g g . A g e G r o u p 
 
 	 	 , a g g . S u b j e c t 
 
 	 	 , a g g . S e c t o r S u b j e c t A r e a 
 
 
 
 f r o m   ( 
 
 
 
 	 	 s e l e c t 	 q u a l . Q a n   a s   Q u a l i f i c a t i o n R e f e r e n c e 
 
 	 	 	 	 , a o . N a m e L e g a l   a s   A w a r d i n g O r g a n i s a t i o n 
 
 	 	 	 	 , q u a l . Q u a l i f i c a t i o n N a m e   a s   Q u a l i f i c a t i o n T i t l e 
 
 	 	 	 	 , v e r . T y p e   a s   Q u a l i f i c a t i o n T y p e 
 
 	 	 	 	 , v e r . L e v e l   a s   L e v e l 
 
 	 	 	 	 , c a s e   w h e n   v e r . P r e S i x t e e n   =   1   t h e n   ' <   1 6 ' 
 
 	 	 	 	 	     w h e n   v e r . S i x t e e n T o E i g h t e e n   =   1   t h e n   ' 1 6   -   1 8 ' 
 
 	 	 	 	 	     w h e n   v e r . E i g h t e e n P l u s   =   1   t h e n   ' 1 8 + ' 
 
 	 	 	 	 	     W h e n   v e r . N i n e t e e n P l u s   =   1   t h e n   ' 1 9 + ' 
 
 	 	 	 	 	     e l s e   n u l l 
 
 	 	 	 	 e n d   a s   A g e G r o u p 
 
 	 	 	 	 , v e r . S p e c i a l i s m   a s   S u b j e c t 
 
 	 	 	 	 , v e r . S s a   a s   S e c t o r S u b j e c t A r e a 
 
 	 	 	 	 , R a n k ( )   O V E R   ( P A R T I T I O N   B Y   q u a l . Q a n   O r d e r   b y   v f c . Q u a l i f i c a t i o n V e r s i o n N u m b e r   d e s c )   a s   r _ n 
 
 	 	 
 
 
 
 	 	 f r o m   d b o . Q u a l i f i c a t i o n   q u a l 
 
 
 
 	 	 i n n e r   j o i n 	 	 r e g u l a t e d . Q u a l i f i c a t i o n V e r s i o n s   v e r     o n   q u a l . i d   =   v e r . Q u a l i f i c a t i o n I d 
 
 	 	 i n n e r   j o i n 	 	 r e g u l a t e d . V e r s i o n F i e l d C h a n g e s   v f c   o n   v f c . i d   =   v e r . V e r s i o n F i e l d C h a n g e s I d 
 
 	 	 i n n e r   j o i n 	 	 d b o . A w a r d i n g O r g a n i s a t i o n   a o   o n   a o . I d   =   v e r . A w a r d i n g O r g a n i s a t i o n I d 
 
 	 	 l e f t   o u t e r   j o i n   r e g u l a t e d . L i f e c y c l e S t a g e   l c s   o n   l c s . i d   =   v e r . L i f e c y c l e S t a g e I d 
 
 
 
 	 	 w h e r e   l c s . n a m e   =   ' C h a n g e d '   )   a g g 
 
 
 
 W h e r e   a g g . r _ n   =   1 
 
 
 
 G O 
 
 