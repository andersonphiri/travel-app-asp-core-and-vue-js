<template>
  <v-app>
    <navigation-bar />
    <v-main>
      <router-view />
    </v-main>
  </v-app>
</template>

<script>
import NavigationBar from "@/components/NavigationBar";
export default {
  name: "App",
  components: {
    NavigationBar,
  },

  data: () => ({
    //
  }),
};
</script>
