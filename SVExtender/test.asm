push ebp
mov ebp, esp
sub esp,0C
mov [esp-0C],ecx
xor ebx,ebx
mov eax, roboptrptr
mov eax, [eax+ebx*4]
mov [esp-08], eax
mov [esp-04], ebx
mov ecx, [esp-0C]
mov eax, addrobo
call eax
inc ebx
mov eax, numberofmecha
cmp eax,ebx
jg; to line 7
mov esp, ebp
pop ebp
ret