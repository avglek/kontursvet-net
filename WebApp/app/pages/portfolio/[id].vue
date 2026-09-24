<template>
  <div>
    <PortfolioCaseCardView :id="id" />
  </div>
</template>

<script lang="ts" setup>
const route = useRoute();

const id = Number.parseInt(route.params.id as string);
</script>

<style></style>
