% This buffer is for notes you don't want to save.
% If you want to create a file, visit that file with C-x C-f,
% then enter the text in that file's own buffer.

:- dynamic(user/1, song/1, likes/2).

clear :-
	retractall(user(_)),
	retractall(song(_)),
	retractall(likes(_,_)).

fillDb(Users) :-
	clear,
	assa_users(Users),!.

assa_users([]).
assa_users([[H|[M]]|T]) :-
	assert(user(H)),
	assert(likes(H, M)),
	assa_users(T).


run(Users, N, Mytracks) :-
	clear,
	assa_users(Users),
	same_taste(Mytracks, N),!.

same_taste(Mytracks, N) :-
	findall(Trks, (likes(_, Trks),checkk(Mytracks, N, Trks)) ,Lst),
	flatten(Lst, List),
	compress(List, L),
	findall(X, (member(X, L), \+member(X, Mytracks)), Result),
	compress(Result, Result2),
	write(Result2),!.

checkk(Mytracks, N, L1) :-
	inter(Mytracks, L1, Rl),
	length(Rl, Rlc),
	Rlc > N, !.

inter([], _, []).
inter([H1|T1], L2, [H1|Res]) :-
    member(H1, L2),
    inter(T1, L2, Res).

inter([_|T1], L2, Res) :-
    inter(T1, L2, Res).

not_inter([], _, []).
not_inter([H1|T1], L2, [H1|Res]) :-
    \+ member(H1, L2),
    not_inter(T1, L2, Res).

not_inter([_|T1], L2, Res) :-
    not_inter(T1, L2, Res).

compress([],[]).
compress([X],[X]).
compress([X,X|Xs],Zs) :- compress([X|Xs],Zs).
compress([X,Y|Ys],[X|Zs]) :- X \= Y, compress([Y|Ys],Zs).

flatten([],[]).
flatten(X,[X]) :- not(is_list(X)).
flatten([X|Xs],Zs) :-
	flatten(X,Y),
	flatten(Xs,Ys),
	append(Y,Ys,Zs).
