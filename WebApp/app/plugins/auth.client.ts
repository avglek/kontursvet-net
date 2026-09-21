export default defineNuxtPlugin(() => {
  const { hydrate } = useAuth();
  hydrate();
});
