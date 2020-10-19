# proffy-backend

## Back-end para o projeto Proffy (Rocketseat/Next Level Week 2)

O projeto inicial era escrito em Node/Typescript, este está escrito utilizando ASP.NET Core 3.1 e Entity Framework Core, além de um serviço em Node/Typescript para cadastro e confirmação de e-mail.

Pontos-chave:

  - `DataContext` utiliza SQLite, mas basta alterar o Startup para utilizar outro banco.
  - A autenticação é feita por JWTs ou Chaves de API
      - JWTs:
          - Utiliza-se um JWT para acesso (expira em 15min, por padrão), este guarda o e-mail (chave candidata para um usuário) e é o token padrão para acesso.
          - Para manter-se autenticado, é provido um refresh token (expira em 12h, por padrão), com este é possível obter um JWT de acesso (definido acima). Por padrão, a rota que provê o refresh token configura um cookie `httpOnly` com esse JWT (dessa maneira, não é preciso logar novamente caso o refresh token esteja válido). Dessa forma, no front-end web, é esperado que refresh token esteja num cookie `httpOnly` e o token de acesso esteja na memória (Context, Redux, Vuex...), tornando ataques XSRF e XSS muito difíceis.
    - Chaves de API:
      - Escopo limitado, estas chaves são configuradas e associadas a um usuário.
- Autorização:
  - Role-based: User <= Admin <= SuperAdmin
