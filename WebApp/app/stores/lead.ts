import { defineStore } from 'pinia';

export const useLeadStore = defineStore('lead', () => {
  // state
  const _estimate = ref('');

  //getter
  const estimate = computed(() => _estimate.value);

  // actions
  function setEstimate(value: string) {
    _estimate.value = value;
  }

  function clearEstimate() {
    _estimate.value = '';
  }

  return { estimate, setEstimate, clearEstimate };
});
